using System.Data;
using System.Web.Mvc;

namespace VIETTEL.Models
{
    public class CheckListDataTableModel
    {
        public CheckListDataTableModel()
        {

        }

        public CheckListDataTableModel(string id, DataTable dt, string selectedItems = null)
        {
            Id = id;
            Data = dt;
            SelectedItems = selectedItems;
        }

        public string Id { get; set; }

        public string GroupId { get; set; }
        public DataTable Data { get; set; }
        public string SelectedItems { get; set; }
    }

    public class ChecklistModel
    {
        public string Id { get; set; }

        public SelectList List { get; set; }

        public ChecklistModel(string id, SelectList list)
        {
            Id = id;
            List = list;
        }
        private ChecklistModel()
        {
            List = new SelectList("");
        }

        public static ChecklistModel Default
        {
            get { return new ChecklistModel(); }
        }
    }
}
