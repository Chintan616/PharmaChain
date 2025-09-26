using PharmaChain.Models;

namespace PharmaChain.ViewModels
{
    public class MedicineSuppliersViewModel
    {
        public Medicine Medicine { get; set; } = new Medicine();
        public List<string> SupplierNames { get; set; } = new List<string>();
    }

    public class SearchMedicinesViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public List<MedicineSuppliersViewModel> Results { get; set; } = new List<MedicineSuppliersViewModel>();
    }
}