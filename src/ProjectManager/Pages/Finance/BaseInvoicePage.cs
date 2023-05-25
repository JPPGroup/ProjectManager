using CommonDataModels;
using CommonDataServices;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Components;
using ProjectManager.Data;
using System.Collections.Concurrent;

namespace ProjectManager.Pages.Finance
{
    public abstract class BaseInvoicePage : ComponentBase
    {
        [CascadingParameter]
        public TaskStateMachine? TaskState { get; set; }

        protected ConcurrentBag<Invoice>? invoices;
        protected IEnumerable<Invoice>? _orderedInvoices;

        protected bool loading;

        [Inject]
        ProjectService? _projects { get; set; }

        protected override void OnInitialized()
        {
            invoices = new ConcurrentBag<Invoice>();
            _orderedInvoices = new List<Invoice>();
            base.OnInitialized();
        }

        protected async override Task OnParametersSetAsync()
        {
            await LoadUserInvoices();
            await base.OnParametersSetAsync();
        }

        private void UpdateInvoiceList()
        {
            Guard.IsNotNull(invoices);
            _orderedInvoices = invoices.OrderBy(i => i.DueDate);
            StateHasChanged();
        }

        private async Task LoadUserInvoices()
        {
            Guard.IsNotNull(invoices);
            Guard.IsNotNull(_projects);
            invoices.Clear();
            loading = true;
            StateHasChanged();

            await foreach (Invoice i in _projects.GetUnpaidInvoices(GetFilter()))
            {
                invoices.Add(i);
                UpdateInvoiceList();
            }

            loading = false;
            UpdateInvoiceList();
        }

        protected abstract string GetFilter();

    }
}
