using CommonDataModels;
using CommonDataServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace ProjectManager.Data
{
    [INotifyPropertyChanged]
    public partial class TaskStateMachine
    {
        readonly ProjectService _projectService;
        readonly ApplicationDbContext _context;
        readonly IHttpContextAccessor _contextAccessor;

        UserProfile? _user;
        readonly SemaphoreSlim _userLock = new(1);

        public IEnumerable<ProjectResponse> UnassignedProjects => _unassignedProjects;
        public IEnumerable<ProjectResponse> CompletedProjects => _completedProjects;
        public IEnumerable<ProjectStates> ProjectStates { get; set; }

        public IEnumerable<ProjectTask> UserTasks { get { return _userTasks; } }
        //TODO: Change to observables to trigger regens
        private List<ProjectTask> _userTasks;

        public IEnumerable<ProjectTask> DailyUserTasks { get; private set; }
        public IEnumerable<ProjectTask> WeeklyUserTasks { get; private set; }
        public IEnumerable<ProjectTask> OtherUserTasks { get; private set; }

        public IEnumerable<Quote> UserQuotes { get; private set; }

        [ObservableProperty]
        Project? _selectedProject;

        [ObservableProperty]
        ProjectTask? _selectedTask;

        private List<ProjectResponse> _unassignedProjects;
        private List<ProjectResponse> _completedProjects;

        private ILogger<TaskStateMachine> _logger;

        public TaskStateMachine(ProjectService projectService, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<TaskStateMachine> logger)
        {
            _projectService = projectService;
            _context = context;
            _contextAccessor = httpContextAccessor;
            _logger = logger;

            _unassignedProjects = new List<ProjectResponse>();
            ProjectStates = new List<ProjectStates>();
            _userTasks = new List<ProjectTask>();

            DailyUserTasks = new List<ProjectTask>();
            WeeklyUserTasks = new List<ProjectTask>();
            OtherUserTasks = new List<ProjectTask>();

            UserQuotes = new List<Quote>();
        }

        public async Task<UserProfile> GetUserAsync(bool nocache = false)
        {
            try
            {
                await _userLock.WaitAsync();

                if (_user != null && !nocache)
                    return _user;

                string? userName = _contextAccessor?.HttpContext?.User?.Identity?.Name;
                if (userName == null)
                    throw new InvalidOperationException("Username not found");

                return await _context.Users.FirstAsync(u => u.UserName == userName);
            }
            finally
            {
                _userLock.Release();
            }
        }

        public UserProfile? GetUser(bool nocache = false)
        {
            try
            {
                _userLock.Wait();
                if (_user != null && !nocache)
                    return _user;

                string? userName = _contextAccessor?.HttpContext?.User?.Identity?.Name;
                if (userName == null)
                    return null;

                return _context.Users.First(u => u.UserName == userName);
            }
            finally
            {
                _userLock.Release();
            }
        }

        public async Task SyncProjects()
        {
            await _context.SaveChangesAsync();

            _user ??= await GetUserAsync();

            //Is this efficient?
            //TODO: Review exectuion
            //TODO: This warns of query splitting for EF
            var allStates = await _context.ProjectStates.Where(ps => ps.UserId == _user.Id).ToListAsync();
            ProjectStates = allStates.Where(ps => ps.State == State.Active);

            _unassignedProjects = new List<ProjectResponse>();
            _completedProjects = new List<ProjectResponse>();

            await foreach (ProjectResponse pr in _projectService.GetProjectsAsync(_user.FirstName, _user.LastName))
            {
                var state = allStates.FirstOrDefault<ProjectStates>(ps => ps.Project.ProjectId == pr.Code);

                if (state == null && (pr.Status == ProjectStatus.Enquiry || pr.Status == ProjectStatus.Live))
                {
                    _unassignedProjects.Add(pr);
                    continue;
                }

                if (state != null && state.State == State.Unknown)
                {
                    _unassignedProjects.Add(pr);
                    continue;
                }

                if (pr.Status == ProjectStatus.Completed || pr.Status == ProjectStatus.Abandoned)
                {

                    if (state != null && state.State == State.Active)
                        _completedProjects.Add(pr);
                }
            }

            await GenerateTaskList();
            //var unmatchedProjects = await _context.ProjectStates.Where(p => results.All(r => r.Code != p.Project.ProjectId)).ToListAsync();

            UserQuotes = _context.Quotes.Where(q => q.IssuerId == _user.Id);
            foreach (Quote quote in UserQuotes)
            {
                _context.Entry(quote).Reference(q => q.Project).Load();
            }
        }

        private async Task GenerateTaskList()
        {
            _user ??= await GetUserAsync();

            _userTasks.Clear();

            //TODO: Switch to use project direct
            foreach (ProjectStates ps in ProjectStates)
            {
                if (ps.State == State.Active)
                    _userTasks.AddRange(ps.Project.Tasks);
            }

            _userTasks = _userTasks.Where(t => t.Status == ProjectTaskStatus.InProgress).OrderBy(t => t.Due).ToList();
            DailyUserTasks = _userTasks.Where(t => t.DaysTillDue <= 1).OrderBy(t => t.Due).ToList();
            WeeklyUserTasks = _userTasks.Where(t => { double x = t.DaysTillDue; return x > 1 && x <= 7; }).OrderBy(t => t.Due).ToList();
            OtherUserTasks = _userTasks.Where(t => t.DaysTillDue > 7).OrderBy(t => t.Due).ToList();
        }

        public async Task AddProject(ProjectResponse response, bool sync = true)
        {
            _user ??= await GetUserAsync();

            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == response.Code) ?? createNewProject(response);
            var state = project.States.FirstOrDefault(ps => ps.UserId == _user.Id) ?? createState(project);

            state.State = State.Active;
            if (sync)
            {
                await _context.SaveChangesAsync();
                await SyncProjects();
            }
        }

        public async Task IgnoreProject(ProjectResponse response, bool sync = true)
        {
            _user ??= await GetUserAsync();

            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == response.Code) ?? createNewProject(response);
            var state = project.States.FirstOrDefault(ps => ps.UserId == _user.Id) ?? createState(project);

            state.State = State.Ignored;
            if (sync)
            {
                await _context.SaveChangesAsync();
                await SyncProjects();
            }
        }

        private Project createNewProject(ProjectResponse response)
        {
            Project project = new Project()
            {
                Id = Guid.NewGuid(),
                ProjectId = response.Code,
                Name = response.Name
            };

            _context.Projects.Add(project);
            return project;
        }

        private ProjectStates createState(Project project)
        {
            if (_user == null)
                _user = GetUser();

            ProjectStates state = new ProjectStates()
            {
                Id = Guid.NewGuid(),
                Project = project,
                User = _user,
                State = State.Unknown
            };

            _context.ProjectStates.Add(state);
            return state;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void DiscardChanges()
        {

        }

        public async Task CompleteProject(ProjectResponse response, bool sync = true)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == response.Code) ?? createNewProject(response);
            await CompleteProject(project, sync);
        }

        public async Task CompleteProject(Project project, bool sync = true)
        {
            _user ??= await GetUserAsync();

            var state = project.States.FirstOrDefault(ps => ps.UserId == _user.Id) ?? createState(project);

            state.State = State.Archived;
            if (sync)
            {
                await _context.SaveChangesAsync();
                await SyncProjects();
            }

            //TODO: Tweak this to use obersvable properly
            if (SelectedProject.ProjectId == project.ProjectId)
                SelectedProject = null;
        }
    }
}
