using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace DynamicResizeAndXml.Workspaces
{
    public partial class UForm_WorkForm : Form
    {
        // переменные для динамического изменения формы
        private Size OriginalFormSize;  // исходный размер формы
        private Rectangle Button1OriginalRectangle; // Исходный размер и координаты элемента управления
        private Rectangle TextBox1OriginalRectangle; // Исходный размер и координаты элемента управления

        private int XmlId;  // переменная используется для открытия нужного файла xml из каталога

        public UForm_WorkForm()
        {
            InitializeComponent();
        }

        // Конструктор используется при переходе между разными рабочими областями в listbox
        public UForm_WorkForm(int SelectedObject)
        {
            InitializeComponent();

            XmlId = SelectedObject;

            LoadDataFromXml();
        }

        // dynamic resize form
        // -------------------------------------------------------------------------------------------
        // При срабатывании события загрузки, переменные инициализируются начальными значениями соответствующими файлу designer
        private void UForm_WorkForm_Load(object sender, EventArgs e)
        {
            OriginalFormSize = this.Size;
            Button1OriginalRectangle = new Rectangle(buttonSave.Location.X, buttonSave.Location.Y, buttonSave.Width, buttonSave.Height);
            TextBox1OriginalRectangle = new Rectangle(textBox1.Location.X, textBox1.Location.Y, textBox1.Width, textBox1.Height);
        }
        // Функция изменения размеров элементов управления пропорционально размеру окна
        private void ResizeControl(Rectangle rectangle, Control control)
        {
            float xRatio = (float)(this.Width) / (float)(OriginalFormSize.Width);
            float yRatio = (float)(this.Height) / (float)(OriginalFormSize.Height);

            int NewX = (int)(rectangle.X * xRatio);
            int NewY = (int)(rectangle.Y * yRatio);

            int NewWidth = (int)(rectangle.Width * xRatio);
            int NewHeight = (int)(rectangle.Height * yRatio);

            control.Location = new Point(NewX, NewY);
            control.Size = new Size(NewWidth, NewHeight);
        }
        // При сробатывании события изменения размера формы, для каждого элемента управления вызывается функция изменения размера
        private void UForm_WorkForm_Resize(object sender, EventArgs e)
        {
            ResizeControl(Button1OriginalRectangle, buttonSave);
            ResizeControl(TextBox1OriginalRectangle, textBox1);
        }
        // -------------------------------------------------------------------------------------------

        // Working with XML
        // -------------------------------------------------------------------------------------------
        // Функция сохранения в XML текста из textBox
        private void SaveDataInXML()
        {
            string Path = $@"D:\Progi\C#\WindowsForms\DynamicResizeAndXml\DynamicResizeAndXml\XMLFiles\File{XmlId}.xml";

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(Path);

            XmlElement Root = XmlDoc.DocumentElement;

            Root.ChildNodes.Item(0).ChildNodes.Item(0).InnerText = textBox1.Text;

            XmlDoc.Save(Path);
        }
        // Функця загрузки данных из XML в textBox
        public void LoadDataFromXml()
        {
            string Path = $@"D:\Progi\C#\WindowsForms\DynamicResizeAndXml\DynamicResizeAndXml\XMLFiles\File{XmlId}.xml";

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(Path);

            XmlElement Root = XmlDoc.DocumentElement;

            textBox1.Text = Root.ChildNodes.Item(0).ChildNodes.Item(0).InnerText;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveDataInXML();
        }
        // -------------------------------------------------------------------------------------------
    }
}
