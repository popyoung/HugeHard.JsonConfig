# Description
A loading/saving config tool for Winform using json, based on Newtonsoft.Json.

# How to use?
1. Add a new config class to your main form.
    ```csharp
    public partial class Form1
    {
        JsonHelper<Config, Form1> Json;
        public class Config : JsonConfig<Form1>
        {
            //In your config file, 'myText' is the key, and textBox1.Text will be the value.
            public TextControlHelper MyText = new TextControlHelper(x => x.textBox1);

            //You can also save an independent item.
            public int Interval { get; set; } = 180;
            
            //With the benefit of Newtonsoft.Json, many different types can be serialized.
            public List<string> Msg { get; set; } = new List<string>();
        }
    }
    ```
1. Call the load function inside the main form constructor.
    ```csharp
    public Form1()
        {
            InitializeComponent();
            JsonHelper = new JsonHelper<JsonConfig, Form1>(this);
            if (!JsonHelper.Load())
                MessageBox.Show("Oops! Can't load the config, a new one will be generated when you close the main form!");
        }
    ```
1. Call the save function when the main form is closing. Maybe you should also do this just after you modify an important item, because if the process is killed, the FormClosing event won't be triggered.
    ```csharp
    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            JsonHelper.Save();
        }
    ```

