using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HugeHard.JsonConfig
{
    public abstract class JsonConfig<TForm>
    {
        [JsonIgnore]
        protected TForm Host;

        public void SetHost(TForm host)
        {
            Host = host;
            foreach (var field in GetType().GetFields())
            {
                if (field.GetValue(this) is IHelper helper)
                    helper.SetHost(host);
            }
        }

        public class NumericUpDownHelper : Helper<NumericUpDown>
        {
            public NumericUpDownHelper(Func<TForm, NumericUpDown> setHost) : base(setHost) { }

            public decimal Value
            {
                get => Control.Value;
                set => Control.Value = value;
            }
        }

        public class CheckBoxHelper : Helper<CheckBox>
        {
            public CheckBoxHelper(Func<TForm, CheckBox> setHost) : base(setHost) { }

            public bool Value
            {
                get => Control.Checked;
                set => Control.Checked = value;
            }
        }

        public class TextControlHelper : Helper<Control>
        {
            public TextControlHelper(Func<TForm, Control> setHost) : base(setHost) { }

            public string Text
            {
                get => Control.Text;
                set => Control.Text = value;
            }
        }

        public class ComboBoxHelper : Helper<ComboBox>
        {
            public ComboBoxHelper(Func<TForm, ComboBox> setHost) : base(setHost) { }

            public int SelectedIndex
            {
                get => Control.SelectedIndex;
                set
                {
                    if (value < Control.Items.Count)
                        Control.SelectedIndex = value;
                    else
                        Control.SelectedIndex = 0;
                }
            }
        }

        public abstract class Helper<TControl> : IHelper
        {
            protected TControl Control;
            private readonly Func<TForm, TControl> setHost;

            public virtual void SetHost(TForm host)
            {
                Control = setHost(host);
            }

            protected Helper(Func<TForm, TControl> setHost)
            {
                this.setHost = setHost;
            }
        }

        public interface IHelper
        {
            void SetHost(TForm host);
        }
    }

    public class JsonHelper<TConfig, TForm> : CustomCreationConverter<TConfig> where TConfig : JsonConfig<TForm>, new()
    {
        protected TForm Host;
        public readonly string FileName;
        public TConfig Config { get; private set; }

        public JsonHelper(TForm host, string fileName = "config.json")
        {
            Host = host;
            FileName = fileName;
        }

        public override TConfig Create(Type objectType)
        {
            TConfig r = new TConfig();
            r.SetHost(Host);
            return r;
        }

        public bool Load()
        {
            try
            {
                var (Success, Obj) = JsonHelper.Load<TConfig>(FileName, this);
                if (Success)
                {
                    Config = Obj;
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Config = new TConfig();
            Config.SetHost(Host);
            return false;
        }

        public void Save()
        {
            JsonHelper.Save(FileName, Config);
        }

    }

    public static class JsonHelper
    {
        public static void Save<T>(string fileName, T obj)
        {
            using (StreamWriter file = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, obj);
            }
        }

        public static (bool Success, T Obj) Load<T>(string fileName, params JsonConverter[] converter) where T : new()
        {
            T result = new T();
            bool success = false;
            try
            {
                if (File.Exists(fileName))
                {
                    string fileTxt = File.ReadAllText(fileName);
                    result = JsonConvert.DeserializeObject<T>(fileTxt, converter);
                    success = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return (success, result);
        }
    }
}


