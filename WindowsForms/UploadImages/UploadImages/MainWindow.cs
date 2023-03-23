using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UploadImages
{
    public partial class MainWindow : Form
    {
        List<PictureBox> ListOfPictureBox = new List<PictureBox>(); // Список всех созданных объектов PictureBox
        PictureBox SelectedItem;    // Выбранный наведением курсора мыши объект PictureBox

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Функция создания контекстного меню
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip CreateContextMenu()
        {
            ContextMenuStrip MyContextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem Increase = new ToolStripMenuItem("Увеличить изображение");
            ToolStripMenuItem Delete = new ToolStripMenuItem("Удалить изображение");

            MyContextMenuStrip.Items.AddRange(new[] { Increase, Delete });

            Increase.Click += new EventHandler(this.IncreaseMenuItem_Click);
            Delete.Click += new EventHandler(this.DeleteMenuItem_Click);

            return MyContextMenuStrip;
        }
        /// <summary>
        /// Событие увеличения изображения по щелчку кнопки из контекстного меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IncreaseMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox LargePictureBox = new PictureBox();
            LargePictureBox.Image = SelectedItem.Image;
            LargePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            Form LargeForm = new Form();

            LargeForm.StartPosition = FormStartPosition.CenterScreen;
            LargeForm.Controls.Add(LargePictureBox);
            LargeForm.Size = new Size(700, 500);

            LargePictureBox.Dock = DockStyle.Fill;

            LargeForm.Show();
        }
        /// <summary>
        /// Событие удаления изображения по щелчку кнопки из контекстного меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            Point DisplacementPoint = new Point(0, 0);
            bool Flag = false;

            for (int i = 0; i < ListOfPictureBox.Count; i++)
            {
                if (ListOfPictureBox[i] == SelectedItem)
                {
                    DisplacementPoint = SelectedItem.Location;
                    panel1.Controls.Remove(SelectedItem);
                    ListOfPictureBox.Remove(SelectedItem);
                    Flag = true;
                }
                if (Flag == true && i != ListOfPictureBox.Count)
                {
                    Point Tmp = ListOfPictureBox[i].Location;
                    ListOfPictureBox[i].Location = DisplacementPoint;
                    DisplacementPoint = Tmp;
                }
            }
        }
        /// <summary>
        /// Событие выбора объекта PictureBox наведением курсора мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            if (sender is PictureBox)
            {
                SelectedItem = (PictureBox)sender;
            }
        }
        /// <summary>
        /// Событие добавления нового изображения по щелчку кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            PictureBox NewPictureBox = new PictureBox();

            // Алгоритм добавления нового PictureBox
            if (ListOfPictureBox.Count == 0)
            {
                NewPictureBox.Height = panel1.Height - 20;
                NewPictureBox.Width = Convert.ToInt32(panel1.Width / 2.5);
                NewPictureBox.Location = new Point(0, 0);
                NewPictureBox.ContextMenuStrip = CreateContextMenu();
                NewPictureBox.MouseEnter += new EventHandler(this.PictureBox_MouseEnter);
            }
            else
            {
                NewPictureBox.Height = panel1.Height - 20;
                NewPictureBox.Width = Convert.ToInt32(panel1.Width / 2.5);
                NewPictureBox.Location = new Point(ListOfPictureBox[ListOfPictureBox.Count - 1].Location.X + ListOfPictureBox[ListOfPictureBox.Count - 1].Width + 50,
                    ListOfPictureBox[ListOfPictureBox.Count - 1].Location.Y);
                NewPictureBox.ContextMenuStrip = CreateContextMenu();
                NewPictureBox.MouseEnter += new EventHandler(this.PictureBox_MouseEnter);
            }

            ListOfPictureBox.Add(NewPictureBox);

            panel1.Controls.Add(NewPictureBox);

            // Открытие файла с изображением из проводника
            OpenFileDialog FileDialog = new OpenFileDialog();

            NewPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                NewPictureBox.Image = Image.FromFile(FileDialog.FileName);
            }

            // Условие отмены создания PictureBox
            if (NewPictureBox.Image == null)
            {
                ListOfPictureBox.Remove(NewPictureBox);
                panel1.Controls.Remove(NewPictureBox);
            }
        }
    }
}
