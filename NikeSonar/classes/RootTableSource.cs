using System;
using System.Collections.Generic;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace NikeSonar
{
    public class RootTableSource : UITableViewSource
    {

        // there is NO database or storage of Tasks in this example, just an in-memory List<>
        NikeStoreAccounts[] tableItems;
        string cellIdentifier = "taskcell"; // set in the Storyboard

        public RootTableSource(NikeStoreAccounts[] items)
        {
            tableItems = items;
        }
        public override int RowsInSection(UITableView tableview, int section)
        {
            return tableItems.Length;
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // in a Storyboard, Dequeue will ALWAYS return a cell, 
            var cell = tableView.DequeueReusableCell(cellIdentifier);
            // now set the properties as normal
            if (tableItems[indexPath.Row].Proxy != "" && tableItems[indexPath.Row].Proxy != null)
            {
                cell.TextLabel.Text = "[P] " + tableItems[indexPath.Row].UserName + " - " +
                                      tableItems[indexPath.Row].Size;
            }
            else
            {
                cell.TextLabel.Text = tableItems[indexPath.Row].UserName + " - " + tableItems[indexPath.Row].Size;
            }
            cell.DetailTextLabel.Text = tableItems[indexPath.Row].Password.Replace("Markos", "password");
            if (tableItems[indexPath.Row].Active)
                cell.Accessory = UITableViewCellAccessory.Checkmark;
            else
                cell.Accessory = UITableViewCellAccessory.None;
            return cell;
        }
        public NikeStoreAccounts GetItem(int id)
        {
            return tableItems[id];
        }
    }
}
