using MyPlanogramDesktopApp;
using MyPlanogramDesktopApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Testing.Customs;
using Testing.Model.Abstracts;

namespace Testing.DB
{
    public static class MainDB 
    {
        public const string ConnectionString = "Server=sql4.freemysqlhosting.net;Database=sql4497931;Uid=sql4497931;Pwd=2Tf1qm9dEG;Allow User Variables=true; AllowZeroDateTime=True; ConvertZeroDateTime=True;default command timeout=10000;";
        public static MySQLDB<Item> DBItem { get; set; } = new();
        public static MySQLDB<Section> DBSection { get; set; } = new();
        public static MySQLDB<Offer> DBOffer { get; set; } = new();
        public static MySQLDB<Barcode> DBBarcode { get; set; } = new();
        public static MySQLDB<Bay> DBBay { get; set; } = new();
        public static MySQLDB<Shelf> DBShelf { get; set; } = new();
        public static MySQLDB<Planogram> DBPlanogram { get; set; } = new();
        public static MySQLDB<Department> DBDepartment { get; set; } = new();

        public static void SetCascadeEvents()
        {
            //MainDB.DBItem.RecordSource.CascadeTriggered += DBItemCascadeTriggered;
            //MainDB.DBSection.RecordSource.CascadeTriggered += DBSectionCascadeTriggered;
            //MainDB.DBBay.RecordSource.CascadeTriggered += DBBayCascadeTriggered;
            //MainDB.DBShelf.RecordSource.CascadeTriggered += DBShelfCascadeTriggered;
            //MainDB.DBOffer.RecordSource.CascadeTriggered += DBOfferCascadeTriggered;
        }

    //    private static void DBOfferCascadeTriggered(object? sender, RecordSource.CascadeEventArgs e) =>
      //  WindowTracker.ItemList.Controller.RemoveRecordsBy("DELETE FROM Item WHERE OfferID=@OfferID;",s=>s.Offer.Equals(e.Record));

      //  private static void DBShelfCascadeTriggered(object? sender, RecordSource.CascadeEventArgs e)=>
    //    WindowTracker.PlanogramList.Controller.RemoveRecordsBy("DELETE FROM Planogram WHERE ShelfID=@ShelfID;",s => s.Shelf.Equals(e.Record));

        //private static void DBBayCascadeTriggered(object? sender, RecordSource.CascadeEventArgs e)=>
  //      WindowTracker.ShelfList.Controller.RemoveRecordsBy("DELETE FROM Shelf WHERE BayID=@BayID;",s =>s.Bay.Equals(e.Record));

        //private static void DBSectionCascadeTriggered(object? sender, RecordSource.CascadeEventArgs e)=>
//        WindowTracker.BayList.Controller.RemoveRecordsBy("DELETE FROM Bay WHERE SectionID=@SectionID;", s =>s.Section.Equals(e.Record));

 //       private static void DBItemCascadeTriggered(object? sender, RecordSource.CascadeEventArgs e)
  //      {
//          WindowTracker.PlanogramList.Controller.RemoveRecordsBy("DELETE FROM Planogram WHERE ItemID=@ItemID;", s => s.Product.Equals(e.Record));
//          WindowTracker.ItemList.Controller.BarcodeListController.RemoveRecordsBy("DELETE FROM Barcode WHERE ItemID=@ItemID;", s => s.Product.Equals(e.Record));
  //      }

        public static async Task FetchData()
        {
            await Task.WhenAll(
                Task.Run(() =>
                {
                    DBPlanogram.OpenConnection();
                    DBItem.OpenConnection();
                    DBOffer.OpenConnection();
                    DBBarcode.OpenConnection();
                    DBSection.OpenConnection();
                    DBBay.OpenConnection();
                    DBShelf.OpenConnection();
                    DBDepartment.OpenConnection();
                }));

            await Task.WhenAll(
                Task.Run(() => DBPlanogram.Select()),
                Task.Run(() => DBShelf.Select()),
                Task.Run(() => DBBay.Select()),
                Task.Run(() => DBSection.Select()),
                Task.Run(() => DBBarcode.Select()),
                Task.Run(() => DBItem.Select()),
                Task.Run(() => DBOffer.Select()),
                Task.Run(() => DBDepartment.Select())
                );

            await Task.WhenAll(
            Task.Run(() =>
            {
                foreach (var record in DBPlanogram.RecordSource)
                {
                    record.SetForeignKeys();
                }
            }),

            Task.Run(() =>
            {
                foreach (var record in DBItem.RecordSource)
                {
                    record.SetForeignKeys();
                }
            }),
            Task.Run(() =>
            {
                foreach (var record in DBBay.RecordSource)
                {
                    record.SetForeignKeys();
                }
            }),
            Task.Run(() =>
            {
                foreach (var record in DBShelf.RecordSource)
                {
                    record.SetForeignKeys();
                }
            }),
            Task.Run(() =>
            {
                foreach (var record in DBBarcode.RecordSource)
                {
                    record.SetForeignKeys();
                }
            }),

            Task.Run(() =>
            {
                SetCascadeEvents();
                DBPlanogram.CloseConnection();
                DBShelf.CloseConnection();
                DBBay.CloseConnection();
                DBSection.CloseConnection();
                DBBarcode.CloseConnection();
                DBItem.CloseConnection();
                DBOffer.CloseConnection();
                DBDepartment.CloseConnection();
            }));
        }


    }
}
