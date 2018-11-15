using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kexi.Common;
using Kexi.Interfaces;
using Kexi.Shell;
using Kexi.UI.Base;
using Kexi.View;
using Kexi.ViewModel;

namespace Kexi.UI.View
{
    public partial class BreadCrumb : BaseControl<BreadcrumbViewModel>
    {
        private string lastValidAdress;

        public BreadCrumb()
        {
            InitializeComponent();
            DataContextChanged += (o, e) =>
            {
                if (ViewModel != null) ViewModel.ModeChanged += ViewModel_ModeChanged;
            };
        }

        private void ViewModel_ModeChanged(object sender, EventArgs e)
        {
            ViewModel.AdressVisible     = ViewModel.Mode == BreadcrumbMode.Adressbox;
            ViewModel.BreadcrumbVisible = !ViewModel.AdressVisible;

            if (ViewModel.Mode == BreadcrumbMode.Adressbox)
            {
                lastValidAdress = text.Text;
                text.SelectAll();
                text.Focus();
            }
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && IsBreadcrumb())
            {
                ViewModel.Mode = BreadcrumbMode.Adressbox;
                e.Handled      = true;
            }
        }

        private async void Text_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    ExitAdressmode();
                    e.Handled = true;
                    break;
                case Key.Return:
                {
                    var path          = text.Text;
                    var currentLister = ViewModel.Workspace.ActiveLister;

                    //TODO: Refactor
                    var protocol      = "file";
                    var protocolIndex = path.IndexOf("://", StringComparison.Ordinal);
                    if (protocolIndex > -1)
                    {
                        protocol = path.Substring(0, protocolIndex).ToLower();
                        path     = path.Substring(protocolIndex + 3);
                    }

                    var lister = ViewModel.Workspace.CreateListerByProtocol(protocol);
                    if (lister != null && lister.ProtocolPrefix != currentLister.ProtocolPrefix)
                    {
                        lister.Path = path;
                        ViewModel.Workspace.ReplaceCurrentLister(lister);
                        await lister.Refresh();
                        ViewModel.Options.RestoreAdressbarVisibility();
                        ViewModel.Mode = BreadcrumbMode.Breadcrumb;
                        e.Handled      = true;
                        return;
                    }

                    e.Handled = true;
                    if (currentLister is IBreadCrumbProvider breadcrumbLister)
                    {
                        if (!await breadcrumbLister.DoBreadcrumbAction(path))
                        {
                            text.Text = lastValidAdress;
                        }
                    }

                    ViewModel.Options.RestoreAdressbarVisibility();
                    ViewModel.Mode = BreadcrumbMode.Breadcrumb;

                    ViewModel.Workspace.FocusCurrentOrFirst();
                    break;
                }
            }
        }

        private bool IsBreadcrumb()
        {
            return ViewModel.Workspace.ActiveLister is IBreadCrumbProvider;
        }

        private void ExitAdressmode()
        {
            if (ViewModel.Mode == BreadcrumbMode.Breadcrumb)
                return;

            ViewModel.Options.RestoreAdressbarVisibility();
            ViewModel.Mode = BreadcrumbMode.Breadcrumb;
            text.Text      = lastValidAdress;
            ViewModel.Workspace.FocusListView();
            ViewModel.Workspace.FocusCurrentOrFirst();
        }

        private void UIElement_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsBreadcrumb())
                return;

            var b     = (Button) sender;
            var path  = (string) b.CommandParameter;
            var dinfo = new[] {new DirectoryInfo(path)};
            var scm   = new ShellContextMenu();
            scm.ShowContextMenu(dinfo, System.Windows.Forms.Cursor.Position); //TODO: System.Windows.Forms && System.Drawing Reference just for this line...
            e.Handled = true;
        }

        private void Text_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ExitAdressmode();
        }
    }
}