using Testing.Controller.AbstractControllers;
using MyPlanogramDesktopApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Testing.DB;
using Testing.RecordSource;
using System.Windows.Controls;

namespace MyPlanogramDesktopApp.Controller
{
    public class ItemFormController : AbstractDataController<Item>
    {
        public OfferListController OfferListController { get; } = new();
        public BarcodeListController BarcodeListController { get; } = new();

        public ItemFormController() {
            AllowNewRecord =false;
            BarcodeListController.ItemForm = true;
            Record = RecordSource.FirstOrDefault();
        }

        public override void SetRecord(Item? record)
        {
            base.SetRecord(record);
            Record.BarcodeFilter = new(Record) {
                                        SelectItem = true
                                        };
            Record.IsDirty= false;
        }

        public override MySQLDB<Item> DB() => MainDB.DBItem;

    }
}
