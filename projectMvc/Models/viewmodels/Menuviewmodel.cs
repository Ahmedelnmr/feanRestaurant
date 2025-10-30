using projectMvc.Models.models;

namespace projectMvc.Models.viewmodels
{
    public class Menuviewmodel
    {
        public List<Category> Categories { get; set; } 
        public List<ManuItem> manuItems { get; set; }
        public int ?SelectedCategoryId { get; set; }

    }
}
