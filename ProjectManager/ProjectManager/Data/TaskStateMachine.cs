using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Data.ProjectIntegration;

namespace ProjectManager.Data
{
    [INotifyPropertyChanged]
    public partial class TaskStateMachine
    {
        ProjectService _projectService;
        ApplicationDbContext _context;
        IHttpContextAccessor _contextAccessor;

        UserProfile? _user;

        public IEnumerable<ProjectResponse> UnassignedProjects { get; set; }
        public IEnumerable<ProjectStates> ProjectStates { get; set; }

        [ObservableProperty]
        Project _selectedProject;

        public TaskStateMachine(ProjectService projectService, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            /*string userName = httpContextAccessor.HttpContext.User.Identity.Name;
            User = context.Users.First(u => u.Id == userName);*/

            _projectService = projectService;
            _context = context;
            _contextAccessor = httpContextAccessor;

            UnassignedProjects = new List<ProjectResponse>();
            ProjectStates = new List<ProjectStates>();
        }

        public async Task<UserProfile> GetUser()
        {
            string userName = _contextAccessor.HttpContext.User.Identity.Name;
            return await _context.Users.FirstAsync(u => u.UserName == userName);
        }

        public async Task SyncProjects()
        {
            await _context.SaveChangesAsync();

            if (_user == null)
                _user = await GetUser();

            var results = await _projectService.GetProjects(_user.FirstName, _user.LastName);
            //Is this efficient?
            //TODO: Review exectuion
            var allStates = await _context.ProjectStates.Where(ps => ps.UserId == _user.Id).ToListAsync();
            ProjectStates = allStates.Where(ps => ps.State == State.Active);
            //var unmatchedProjects = await _context.ProjectStates.Where(p => results.All(r => r.Code != p.Project.ProjectId)).ToListAsync();

            UnassignedProjects = results.Where(r => ProjectStates.All(ps => r.Code != ps.Project.ProjectId && ps.State == State.Unknown)).ToList();
        }

        public async Task AddProject(ProjectResponse response, bool sync = true)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == response.Code);
            if (project == null)
                project = createNewProject(response);

            var state = project.States.FirstOrDefault(ps => ps.UserId == _user.Id);
            if (state == null)
                state = createState(project);

            state.State = State.Active;
            if (sync)
            {
                await _context.SaveChangesAsync();
                await SyncProjects();
            }
        }

        public async Task IgnoreProject(ProjectResponse response, bool sync = true)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == response.Code);
            if (project == null)
                project = createNewProject(response);

            var state = project.States.FirstOrDefault(ps => ps.UserId == _user.Id);
            if (state == null)
                state = createState(project);

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
    }
}
