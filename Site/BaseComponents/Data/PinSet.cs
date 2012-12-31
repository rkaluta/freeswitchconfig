using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.Dbpro.Structure.Attributes;
using Org.Reddragonit.Dbpro.Connections;
using Org.Reddragonit.Dbpro.Connections.Parameters;
using System.Threading;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Attributes;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Core;
using Org.Reddragonit.FreeSwitchConfig.DataCore.System.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Events;
using Org.Reddragonit.FreeSwitchConfig.DataCore;
using Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.DialPlans;
using Org.Reddragonit.BackBoneDotNet.Attributes;
using Org.Reddragonit.BackBoneDotNet.Interfaces;
using Org.Reddragonit.FreeSwitchConfig.DataCore.PhoneSystem;
using Org.Reddragonit.FreeSwitchConfig.DataCore.Generators;
using Org.Reddragonit.Dbpro.Connections.ClassSQL;
using System.Data;
using Org.Reddragonit.BackBoneDotNet;
using Org.Reddragonit.FreeSwitchConfig.DataCore.DB.Phones;

namespace Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data
{
    [Table()]
    [ModelJSFilePath("/resources/scripts/Core/PBXConfiguration/PinSets.js")]
    [ModelRoute("/core/models/pbxConfig/PinSet")]
    [ModelBlockJavascriptGeneration(ModelBlockJavascriptGenerations.EditForm | ModelBlockJavascriptGenerations.View | ModelBlockJavascriptGenerations.CollectionView)]
    [ModelNamespace("FreeswitchConfig.PBX")]
    public class PinSet : Org.Reddragonit.Dbpro.Structure.Table,IModel
    {
        private string _orignalName;
        private string _name;
        [PrimaryKeyField(false, 15)]
        public string Name
        {
            get { return _name; }
            set {
                _orignalName = (_orignalName == null ? value : _orignalName);
                _name = value; 
            }
        }

        private Context _context;
        [ForeignPrimaryKeyField(ForeignPrimaryKeyField.UpdateDeleteAction.CASCADE, ForeignField.UpdateDeleteAction.CASCADE)]
        public Context Context
        {
            get { return _context; }
            set { _context = value; }
        }

        private string _description;
        [Field(250,false)]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private bool _advanced = false;
        [Field(false)]
        public bool Advanced
        {
            get { return _advanced; }
            set { _advanced = value; }
        }

        [ModelLoadAllMethod()]
        public static List<PinSet> LoadAll()
        {
            List<PinSet> ret = new List<PinSet>();
            Connection conn = ConnectionPoolManager.GetConnection(typeof(PinSet)).getConnection();
            foreach (PinSet ps in conn.Select(typeof(PinSet),
                new SelectParameter[]{
                    new EqualParameter("Context",Context.Current)
                }))
                ret.Add(ps);
            conn.CloseConnection();
            return ret;
        }

        [ModelLoadMethod()]
        public static PinSet Load(string id)
        {
            PinSet ret = null;
            Connection conn = ConnectionPoolManager.GetConnection(typeof(PinSet)).getConnection();
            List<Org.Reddragonit.Dbpro.Structure.Table> tmp = conn.Select(typeof(PinSet),
                new SelectParameter[] { new EqualParameter("Name", (id.Contains("@") ? id.Substring(0,id.IndexOf("@")) : id)),
                new EqualParameter((id.Contains("@") ? "Context.Name" : "Context"),(id.Contains("@") ? (object)id.Substring(id.IndexOf("@")+1) : (object)Context.Current))});
            if (tmp.Count > 0)
                ret = (PinSet)tmp[0];
            conn.CloseConnection();
            return ret;
        }

        [ModelSelectListMethod()]
        public static List<sModelSelectOptionValue> SelectList()
        {
            List<sModelSelectOptionValue> ret = new List<sModelSelectOptionValue>();
            ClassQuery cq = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data",
                "SELECT ps.Name,ps.Context.Name FROM PinSet ps WHERE ps.Context.Name = @contextName");
            cq.Execute(new IDbDataParameter[]{
                cq.CreateParameter("@contextName",Context.Current.Name)
            });
            while (cq.Read())
                ret.Add(new sModelSelectOptionValue(cq[0].ToString() + "@" + cq[1].ToString(), cq[0].ToString()));
            cq.Close();
            return ret;
        }

        private List<NameValuePair> _pins;
        public NameValuePair[] Pins
        {
            get {
                if (_pins == null)
                {
                    _pins = new List<NameValuePair>();
                    foreach (Pin p in Pin.LoadAllForSet(this))
                    {
                        if (Advanced)
                            _pins.Add(new NameValuePair(p.Extension.id, p.PinNumber));
                        else
                            _pins.Add(new NameValuePair(p.ID.ToString(), p.PinNumber));
                    }
                }
                return _pins.ToArray(); 
            }
            set { _pins = new List<NameValuePair>(value); }
        }

        private object _PinData
        {
            get
            {
                object ret;
                if (Advanced)
                {
                    List<PinnedRoutePlan.sAdvancedPin> apins = new List<PinnedRoutePlan.sAdvancedPin>();
                    ClassQuery cqa = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data",
                        "SELECT pn.PinNumber,pn.Extension.Number,pn.Extension.Domain.Name FROM Pin pn WHERE pn.OwningSet.Name = @setName AND pn.OwningSet.Context.Name = @contextName");
                    cqa.Execute(new IDbDataParameter[]{
                        cqa.CreateParameter("@setName",Name),
                        cqa.CreateParameter("@contextName",Context.Name)
                    });
                    while (cqa.Read())
                        apins.Add(new PinnedRoutePlan.sAdvancedPin(cqa[1].ToString(), cqa[2].ToString(), cqa[0].ToString()));
                    cqa.Close();
                    ret = apins;
                }
                else
                {
                    List<string> spins = new List<string>();
                    ClassQuery cqs = new ClassQuery("Org.Reddragonit.FreeSwitchConfig.Site.BaseComponents.Data",
                        "SELECT pn.PinNumber FROM Pin pn WHERE pn.OwningSet.Name = @setName AND pn.OwningSet.Context.Name = @contextName");
                    cqs.Execute(new IDbDataParameter[]{
                        cqs.CreateParameter("@setName",Name),
                        cqs.CreateParameter("@contextName",Context.Name)
                    });
                    while (cqs.Read())
                        spins.Add(cqs[0].ToString());
                    cqs.Close();
                    ret = spins.ToArray();
                }
                return ret;
            }
        }

        [ModelDeleteMethod()]
        public new bool Delete()
        {
            bool ret = true;
            try
            {
                base.Delete();
                ConfigurationController.RegisterChangeCall(
                            typeof(PinnedRoutePlan),
                            new ADialPlan.sUpdateConfigurationsCall(
                                "DeletePinset",
                            new NameValuePair[]{
                                new NameValuePair("context",Context.Name),
                                new NameValuePair("name",Name)
                            }),
                            new IEvent[]{
                                new GenericEvent("PinSetDeleted",
                                    new NameValuePair[]{
                                        new NameValuePair("Name",Name),
                                        new NameValuePair("Context",Context.Name),
                                        new NameValuePair("IsAdvanced",Advanced)
                                })
                            });
            }
            catch (Exception e)
            {
                Log.Error(e);
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                ret = false;
            }
            return ret;
        }

        [ModelUpdateMethod()]
        public new bool Update()
        {
            bool ret = true;
            try
            {
                base.Update();
                List<Pin> pins = Pin.LoadAllForSet(this);
                for(int x=0;x<pins.Count;x++)
                {
                    Pin p = pins[x];
                    bool found = false;
                    for(int y=0;y<_pins.Count;y++)
                    {
                        NameValuePair nvp = _pins[y];
                        if (Advanced && p.Extension!=null)
                        {
                            if (nvp.Name == p.Extension.id)
                            {
                                found = true;
                                p.PinNumber = nvp.Value.ToString();
                                p.Update();
                                _pins.RemoveAt(y);
                                break;
                            }
                        }
                        else if (!Advanced)
                        {
                            if (p.ID.ToString() == nvp.Name)
                            {
                                found = true;
                                p.PinNumber = nvp.Value.ToString();
                                p.Extension = null;
                                p.Update();
                                _pins.RemoveAt(y);
                            }
                        }
                    }
                    if (found)
                    {
                        pins.RemoveAt(x);
                        x--;
                    }
                    else
                        p.Delete();
                }
                foreach (NameValuePair nvp in _pins)
                {
                    Pin p = new Pin();
                    if (Advanced)
                        p.Extension = Extension.Load(nvp.Name);
                    p.PinNumber = nvp.Value.ToString();
                    p.OwningSet = this;
                    p.Save();
                }
                ConfigurationController.RegisterChangeCall(
                            typeof(PinnedRoutePlan),
                            new ADialPlan.sUpdateConfigurationsCall(
                                (Advanced ? "UpdateAdvancedPinset" : "UpdateBasicPinset"),
                            new NameValuePair[]{
                                new NameValuePair("context",Context.Name),
                                new NameValuePair("name",_orignalName),
                                new NameValuePair("newName",Name),
                                new NameValuePair("pins",_PinData)
                            }),
                            new IEvent[]{
                                new GenericEvent("PinSetUpdated",
                                    new NameValuePair[]{
                                        new NameValuePair("Name",Name),
                                        new NameValuePair("Context",Context.Name),
                                        new NameValuePair("IsAdvanced",Advanced)
                                })
                            });
            }
            catch (Exception e)
            {
                Log.Error(e);
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                ret = false;
            }
            return ret;
        }

        [ModelSaveMethod()]
        public new bool Save()
        {
            bool ret = true;
            try
            {
                base.Save();
                foreach (NameValuePair nvp in _pins)
                {
                    Pin p = new Pin();
                    if (Advanced)
                        p.Extension = Extension.Load(nvp.Name);
                    p.PinNumber = nvp.Value.ToString();
                    p.OwningSet = this;
                    p.Save();
                }
                ConfigurationController.RegisterChangeCall(
                            typeof(PinnedRoutePlan),
                            new ADialPlan.sUpdateConfigurationsCall(
                                (Advanced ? "AddAdvancedPinset" : "AddBasicPinset"),
                            new NameValuePair[]{
                                new NameValuePair("context",Context.Name),
                                new NameValuePair("name",_orignalName),
                                new NameValuePair("pins",_PinData)
                            }),
                            new IEvent[]{
                                new GenericEvent("PinSetCreated",
                                    new NameValuePair[]{
                                        new NameValuePair("Name",Name),
                                        new NameValuePair("Context",Context.Name),
                                        new NameValuePair("IsAdvanced",Advanced)
                                })
                            });
            }
            catch (Exception e)
            {
                Log.Error(e);
                EventController.TriggerEvent(new ErrorOccuredEvent(e));
                ret = false;
            }
            return ret;
        }

        #region IModel Members

        public string id
        {
            get { return Name + "@" + Context.Name; }
        }

        #endregion
    }
}
