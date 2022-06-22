using HugeHard.JsonConfig;

namespace WinFormsAppDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            JsonHelper = new JsonHelper<JsonConfig, Form1>(this);
            if (!JsonHelper.Load())
                MessageBox.Show("Oops! Can't load the config, a new one will be generated when you close the main form!");
        }

        readonly JsonHelper<JsonConfig, Form1> JsonHelper;
        JsonConfig Config => JsonHelper.Config;
        public class JsonConfig : JsonConfig<Form1>
        {
            public TextControlHelper TextWillBeSaved = new TextControlHelper(x => x.textBox1);
            public NumericUpDownHelper NumberWillBeSaved = new NumericUpDownHelper(x => x.numericUpDown1);
            public CheckBoxHelper CheckState1 = new CheckBoxHelper(x => x.checkBox1);
            public CheckBoxHelper CheckState2 = new CheckBoxHelper(x => x.checkBox2);
            public ComboBoxHelper ComboBoxSelectedIndex = new ComboBoxHelper(x => x.comboBox1);
            public string? IndependentData { get; set; } = "Hello world!";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            JsonHelper.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine(Config.IndependentData);
            Console.WriteLine("Typing a new string to be saved:");
            Config.IndependentData = Console.ReadLine();
        }
    }
}