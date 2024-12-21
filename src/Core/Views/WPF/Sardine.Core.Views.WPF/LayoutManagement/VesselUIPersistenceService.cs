using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;
using Sardine.Core.FileManagement;
using Sardine.Core.Utils.Reflection;

namespace Sardine.Core.Views.WPF.LayoutManagement
{
    public static class VesselUIPersistenceService
    {
        public static bool Load(SardineWindow? managerUI, FileStream stream)
        {
            if (managerUI is null)
                return false;

            managerUI.CloseAllVesselUIs();

            XDocument document = XDocument.Load(stream);

            foreach (XElement uiElementInfo in document.Root!.Elements())
            {
                Vessel? vessel = Fleet.Current.VesselCollection.Where((x) => x.Name == uiElementInfo.Attribute("Vessel")!.Value).FirstOrDefault();

                if (vessel is null)
                    continue;


                string? typeString = uiElementInfo.Attribute("Type")?.Value;

                Type? type = (typeString is null) ? null : Type.GetType(typeString);

                double left = Convert.ToDouble(uiElementInfo.Attribute("Left")?.Value ?? "0", CultureInfo.InvariantCulture);
                double top = Convert.ToDouble(uiElementInfo.Attribute("Top")?.Value ?? "0", CultureInfo.InvariantCulture);
                bool locked = Convert.ToBoolean(uiElementInfo.Attribute("Locked")?.Value ?? "false", CultureInfo.InvariantCulture);

                managerUI.OpenVesselUI(vessel, type, (left, top), locked: locked);


                foreach (XElement uiElement in uiElementInfo.Elements())
                {
                    foreach (XAttribute uiAttribute in uiElement.Attributes())
                    {
                        PropertyInfo property = vessel.HandleType.GetProperty(uiAttribute.Name.LocalName)!;

                        if (vessel.IsOnline)
                        {
                            object convertedPropertyValue;

                            if (property.PropertyType.IsEnum)
                                convertedPropertyValue = Enum.Parse(property.PropertyType, uiAttribute.Value);
                            else
                                convertedPropertyValue = Convert.ChangeType(uiAttribute.Value, property.PropertyType, CultureInfo.InvariantCulture);

                            property.SetValue(vessel.ObjectHandle, convertedPropertyValue);
                        }
                    }
                }

            }

            return true;
        }

        public static void Save(SardineWindow? managerUI)
        {
            if (managerUI is null)
                return;

            string? filename = SaveSardineFileDialog.GetPath("SARDINE Layout", FileType.OfType<VesselUILayoutFileType>());

            if (filename is null)
                return;

            if (string.IsNullOrEmpty(filename))
                return;

            XDocument document = new(new XElement("SardineUILayout", new XAttribute("AggregatorType", AssemblyInformation.EntryAssemblyName)));

            foreach (KeyValuePair<Dispatcher, object?> kvp in managerUI.Dispatchers)
            {
                if (kvp.Value is VesselUIWindow window)
                {
                    XElement element = new("VesselUI");

                    string name = string.Empty;
                    double left=0;
                    double top=0;
                    bool locked = false;
                    (object, Vessel, Type)[] viewModels = [];
                    
                    // getting relevant window info
                    kvp.Key.Invoke(() =>
                    {
                        name = window.Vessel.Name;
                        left = window.Left;
                        top = window.Top;
                        locked = window.Locked;
                        viewModels = window.UIContainer.VesselUIs.Select( x=> (x.VesselUI.DataContext, x.VesselUI.Vessel!,x.UIType)).ToArray();
                    });

                    element.Add(new XAttribute("Vessel", name));
                    element.Add(new XAttribute("Top", top));
                    element.Add(new XAttribute("Left", left));
                    element.Add(new XAttribute("Locked", locked));

                    if (viewModels.Length == 1)
                        element.Add(new XAttribute("Type", viewModels[0].Item3.AssemblyQualifiedName ?? viewModels[0].Item3.FullName ?? viewModels[0].Item3.Name));

                    foreach((object ViewModel, Vessel Vessel, Type UIType) vmtVP in viewModels)
                    {
                        XElement uiData = new(vmtVP.UIType.Name);
                        
                        var propNameVesselList = vmtVP.Vessel.HandleType.GetProperties().Where(x2 => x2.SetMethod is not null && x2.SetMethod.IsPublic && x2.GetMethod is not null && x2.GetMethod.IsPublic && x2.PropertyType.IsValueType).Select(x=>x.Name);                        
                        IEnumerable<PropertyInfo> propList = vmtVP.ViewModel.GetType().GetProperties().Where(x => propNameVesselList.Contains(x.Name));

                        foreach (PropertyInfo prop in propList)
                            uiData.Add(new XAttribute(prop.Name, prop.GetValue(vmtVP.ViewModel)!));

                        element.Add(uiData);
                    }

                    document.Root!.Add(element);
                }
            }

            FileManager.Save(Encoding.UTF8.GetBytes(document.ToString()), filename, FileType.OfType<VesselUILayoutFileType>());
        }
    }
}
