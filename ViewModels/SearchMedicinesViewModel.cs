using PharmaChain.Models;

namespace PharmaChain.ViewModels
{
    public class SearchMedicinesViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public List<Medicine> Medicines { get; set; } = new List<Medicine>();
    }
}
