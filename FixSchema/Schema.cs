using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FixSchema
{
    public class Schema
    {
        XDocument fixml;

        public Schema()
        {
            Initialise();            
        }

        private void Initialise()
        {
            fixml = XDocument.Load("FIX42.xml");
            LoadFields();
            LoadHeader();
            LoadTrailer();
            LoadMessages();
        }

        private void LoadFields()
        {
            XElement xe = fixml.Root.Element("fields");
            foreach (XElement field in xe.Elements())
            {
                int tag = int.Parse(field.Attribute("number").Value);
                string name = field.Attribute("name").Value;
                string type = field.Attribute("type").Value;
                Field f = new Field(tag, name, type);
                
                if (field.HasElements)
                {
                    foreach (XElement fv in field.Elements())
                    {
                        f.AddFieldValue(fv.Attribute("enum").Value, fv.Attribute("description").Value);
                    }
                }

                Fields.Instance.AddField(f);
            }
        }

        private void LoadHeader()
        {
            XElement xe = fixml.Root.Element("header");
            foreach (XElement field in xe.Elements())
            {
                string name = field.Attribute("name").Value;
                bool required = false;
                if (field.Attribute("required").Value == "Y")
                {
                    required = true;
                }

                Header.Instance.AddMessageField(name, required);

            }
        }

        private void LoadTrailer()
        {
            XElement xe = fixml.Root.Element("trailer");
            foreach (XElement field in xe.Elements())
            {
                string name = field.Attribute("name").Value;
                bool required = false;
                if (field.Attribute("required").Value == "Y")
                {
                    required = true;
                }
                Trailer.Instance.AddMessageField(name, required);
            }
        }

        private void LoadMessages()
        {
            XElement xe = fixml.Root.Element("messages");
            foreach (XElement message in xe.Elements())
            {
                string name = message.Attribute("name").Value;
                string msgtype = message.Attribute("msgtype").Value;
                string msgcat = message.Attribute("msgcat").Value;

                Console.WriteLine("Processing: {0}", name);

                Message fixmessage = new Message(name, msgtype, msgcat);

                foreach (XElement mo in message.Elements())
                {                    
                    if (mo.Name == "field")
                    {
                        string fieldname = mo.Attribute("name").Value;
                        bool required = false;
                        if (mo.Attribute("required").Value == "Y")
                        {
                            required = true;
                        }

                        MessageField mf = new MessageField(fieldname, required);
                        fixmessage.AddMessageObject(mf);
                    }
                    else if (mo.Name == "group")
                    {
                        fixmessage.AddMessageObject(RecurseGroup(mo));
                    }
                }

                Messages.Instance.AddMessage(fixmessage);
            }

        }

        private MessageGroup RecurseGroup(XElement mo)
        {
            string groupname = mo.Attribute("name").Value;
            bool grouprequired = false;
            if (mo.Attribute("required").Value == "Y")
            {
                grouprequired = true;
            }
            
            MessageGroup mg = new MessageGroup(groupname, grouprequired);

            foreach (XElement xe in mo.Elements())
            {
                if (xe.Name == "field")
                {
                    string fieldname = xe.Attribute("name").Value;
                    bool required = false;
                    if (xe.Attribute("required").Value == "Y")
                    {
                        required = true;
                    }

                    MessageField mf = new MessageField(fieldname, required);
                    mg.AddMessageObject(mf);
                }
                else if (xe.Name == "group")
                {
                    mg.AddMessageObject(RecurseGroup(xe));
                }
            }

            return mg;
        }
    }
}
