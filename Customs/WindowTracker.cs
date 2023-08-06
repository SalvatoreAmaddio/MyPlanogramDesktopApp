using MyPlanogramDesktopApp.View;
using MyPlanogramDesktopApp.View.Bay;
using MyPlanogramDesktopApp.View.Item;
using MyPlanogramDesktopApp.View.Offer;
using MyPlanogramDesktopApp.View.Planogram;
using MyPlanogramDesktopApp.View.Section;
using MyPlanogramDesktopApp.View.Shelf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Customs
{
    public static class WindowTracker
    {
        public static ItemList? ItemList { get; set; }
        public static SectionList? SectionList { get; set; }
        public static BayList? BayList { get; set; }
        public static ShelfList? ShelfList { get; set; }
        public static PlanogramList? PlanogramList { get; set; }
        public static OfferList? OfferList { get; set; }
        public static BarcodeList? BarcodeList { get; set; }
        public static PlanViewer WebPage { get; internal set; }
        public static DepartmentView DepartmentView { get; internal set; }
    }
}
