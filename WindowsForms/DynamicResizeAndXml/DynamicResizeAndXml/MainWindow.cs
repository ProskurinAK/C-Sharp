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

namespace DynamicResizeAndXml
{
    public partial class MainWindow : Form
    {
        private Form ActiveWorkspace;   // переменная хранит открытую рабочую область(используется в функции OpenWorkspace)

        // переменные для динамического изменения формы
        // для каждого элемента управления, который будет поддерживать динамическое изменение размера, нужна своя переменная
        private Size OriginalFormSize;  // исходный размер формы
        private Rectangle Panel1OriginalRectangle;  // Исходный размер и координаты элемента управления
        private Rectangle ListBox1OriginalRectangle;    // Исходный размер и координаты элемента управления
        private Rectangle Button1OriginalRectangle; // Исходный размер и координаты элемента управления

        private int XmlId;  // переменная используется для открытия нужного файла xml из каталога

        public MainWindow()
        {
            InitializeComponent();
        }

        // Метод открытия вложенной формы на панели внешней формы
        private void OpenWorkspace()
        {
            if (ActiveWorkspace != null)
            {
                ActiveWorkspace.Close();
            }

            ActiveWorkspace = new Workspaces.UForm_WorkForm(listBox1.SelectedIndex);

            ActiveWorkspace.TopLevel = false;
            ActiveWorkspace.Dock = DockStyle.Fill;
            panel2.Controls.Add(ActiveWorkspace);
            ActiveWorkspace.Show();
        }

        // Функция создания файла XML
        private void CreateXML()
        {
            string Path = $@"D:\Progi\C#\WindowsForms\DynamicResizeAndXml\DynamicResizeAndXml\XMLFiles\File{XmlId}.xml";

            XmlDocument XmlDoc = new XmlDocument();

            XmlDeclaration XmlDec = XmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlDoc.AppendChild(XmlDec);

            XmlElement ElementControls = XmlDoc.CreateElement("Controls");
            XmlDoc.AppendChild(ElementControls);

            XmlElement ElementTextBox1 = XmlDoc.CreateElement("TextBox1");
            ElementControls.AppendChild(ElementTextBox1);

            XmlElement TextBox1Field = XmlDoc.CreateElement("TextField");
            ElementTextBox1.AppendChild(TextBox1Field);

            XmlDoc.Save(Path);

            XmlId++;
        }

        // При нажатии на кнопку добавить - 1) Создаётся новый объект рабочей области
        // 2) Создаётся новый XML файл
        // 3) Объект рабочей области добавляется в listbox
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            Workspaces.UForm_WorkForm Form_WorkForm = new Workspaces.UForm_WorkForm();

            CreateXML();

            listBox1.Items.Add(Form_WorkForm);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                OpenWorkspace();
            }
            catch
            {

            }
        }

        // dynamic resize form
        // -------------------------------------------------------------------------------------------
        // При срабатывании события загрузки, переменные инициализируются начальными значениями соответствующими файлу designer
        private void MainWindow_Load(object sender, EventArgs e)
        {
            OriginalFormSize = this.Size;
            Panel1OriginalRectangle = new Rectangle(panel1.Location.X, panel1.Location.Y, panel1.Width, panel1.Height);
            ListBox1OriginalRectangle = new Rectangle(listBox1.Location.X, listBox1.Location.Y, listBox1.Width, listBox1.Height);
            Button1OriginalRectangle = new Rectangle(ButtonAdd.Location.X, ButtonAdd.Location.Y, ButtonAdd.Width, ButtonAdd.Height);
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
        private void MainWindow_Resize(object sender, EventArgs e)
        {
            ResizeControl(Panel1OriginalRectangle, panel1);
            ResizeControl(ListBox1OriginalRectangle, listBox1);
            ResizeControl(Button1OriginalRectangle, ButtonAdd);
        }
        // -------------------------------------------------------------------------------------------
    }
}
