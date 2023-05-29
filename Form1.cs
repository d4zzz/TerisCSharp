using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class MainForm : Form
    {
        private Brush fixedBrickColor = Brushes.Green, movingBrickColor = Brushes.Black;
        private Canvas canvas;
        

        public MainForm()
        {
            InitializeComponent();
        }
        // 载入窗体并开始游戏
        private void MainForm_Load(object sender, EventArgs e)
        {
            
            StartGame();
        }
        //处理击键事件，需要将窗口的KeyPreview设置为true
        //否则windows窗口将优先捕获击键事件，并且不会向后传递
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Down:
                    GameTimer.Interval = 50;
                    break;
                case Keys.Up:
                    canvas.BrickRotate();
                    break;
                case Keys.Right: 
                    canvas.BrickMove(Brick.DIRECTION.RIGHT); 
                    break;
                case Keys.Left:
                    canvas.BrickMove(Brick.DIRECTION.LEFT); 
                    break;
                default:
                    return;
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // 计时器处理函数，返回false则无法继续游戏
            if (!canvas.Tick())
            {
                StopGame();
            }

        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    GameTimer.Interval = 800;
                    break;
            }
            }

        private void StartGame()
        {
            //开启计时器
            GameTimer.Interval = 800;
            GameTimer.Start();
            //新建游戏画布，确定行、列方块数
            canvas = new Canvas(GamePictureBox, GamePictureBox.Size.Height/Canvas.cube_width, GamePictureBox.Width/Canvas.cube_width, fixedBrickColor, movingBrickColor);
            //开始游戏
            canvas.StartGame();
        }
        //停止游戏
        private void StopGame()
        {
            // 停止计时器
            GameTimer.Stop();
            // 生成弹窗
            DialogResult boxResult = MessageBox.Show("游戏结束，是否继续游戏","",MessageBoxButtons.YesNo);
            if (boxResult == DialogResult.Yes)
            {
                StartGame();
                return;
            }
            // 退出程序
            Environment.Exit(0);
        }
    }
}
