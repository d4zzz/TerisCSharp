using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tetris
{
    public class Canvas
    {
        
        private Brick brick_current = default, brick_next = default;

        private int[,] canvas_data;
        
        public const int cube_width = 20,brick_width = 5;

        //游戏绘图控件
        private PictureBox canvasBox = default;
        private Brush fixedBrickColor, movingBrickColor;

        private int canvas_row, canvas_column;
        public Canvas(PictureBox picturBox,int row,int column,Brush fColor,Brush mColor)
        {
            canvasBox = picturBox;
            canvas_row = row;
            canvas_column = column;
            canvas_data = new int[row,column];
            fixedBrickColor = fColor;
            movingBrickColor = mColor;
        }
        //方块左右移动
        public bool  BrickMove(Brick.DIRECTION direct = Brick.DIRECTION.DOWN)
        {
            Point next_loc = default;

            //判断下个位置是否碰撞
            switch (direct)
            {
                case Brick.DIRECTION.LEFT:
                    next_loc = new Point(brick_current.Location.X - 1, brick_current.Location.Y);
                    if (!brick_current.CollisionDetect(next_loc,brick_current.Data,canvas_data))
                    {
                        brick_current.Location = next_loc;
                        Paint();
                        return true;
                    }
                    
                    break;
                case Brick.DIRECTION.RIGHT:

                    next_loc = new Point(brick_current.Location.X + 1, brick_current.Location.Y);
                    if (!brick_current.CollisionDetect(next_loc,brick_current.Data,canvas_data))
                    {
                        brick_current.Location = next_loc;
                        Paint();
                        return true;
                    }
                    
                    break;
                case Brick.DIRECTION.DOWN:
                    // 方块垂直下落
                    next_loc = new Point(brick_current.Location.X, brick_current.Location.Y + 1);
                    if (!brick_current.CollisionDetect(next_loc, brick_current.Data, canvas_data))
                    {
                        brick_current.Location = next_loc;
                        Paint();
                        return true;
                    }
                    else
                    {
                        //如果碰到底部方块或者底线，合并到底部方块矩阵
                        BrickMerge();
                        Paint();
                        break;
                    }
                    

            }
            return false;
        }
        
        //旋转俄罗斯方块
        public bool BrickRotate() 
        {
            if (brick_current.Rotate(canvas_data))
            {
                Paint();
                return true;
            }
            
            return false;
        }
        //合并当前俄罗斯方块和已有方块
        private void BrickMerge()
        {
            for(int i = 0;i < brick_width;i++)
            {
                for(int j = 0;j < brick_width;j++  )
                {
                    if (brick_current.Data[i,j] == 1)
                    {
                        canvas_data[brick_current.Location.Y + i, brick_current.Location.X + j] = 1;
                    }
                }
            }
        }
        //生成新的俄罗斯方块
        public bool GenNewBrick()
        {
            //随机生成方块类型
            switch (new Random().Next(0, 5))
            {
                case (int)Brick.BRICK_TYPE.BRICK_BAR:
                    brick_current = new BrickBar();
                    
                    break;
                case (int)Brick.BRICK_TYPE.BRICK_T:
                    brick_current = new BrickT();
                    
                    break;
                case (int)Brick.BRICK_TYPE.BRICK_L:
                    brick_current = new BrickL();
                    
                    break;
                case (int)Brick.BRICK_TYPE.BRICK_J:
                    brick_current = new BrickJ();
                    
                    break;
                case (int)Brick.BRICK_TYPE.BRICK_SQURE:
                    brick_current = new BrickSqure();
                   
                    break;
                case (int)Brick.BRICK_TYPE.BRICK_Z:
                    brick_current = new BrickZ();
                    break;
            }
            // 新生成的方块位置是画布最上方的中央：（画布长度-方块矩阵长度）/2
            brick_current.Location = new Point((canvas_column - brick_width )/ 2,0);
            Paint();
            // 新生成的方块进行初始化，检测是否能够容纳新方块
            if (brick_current.Init(canvas_data))
            {
                return true;
            }
            return false;   
        }
        // TODO：显示即将出现的下一个方块
        public void NextBrick() 
        {
            
        }
        //画布绘图
        public void Paint() 
        {
            //新建bitmap
            Bitmap canvas = new Bitmap(canvas_column * cube_width, canvas_row * cube_width);
            //创建Graphics对象
            Graphics g = Graphics.FromImage(canvas);
            
            //先绘制绘制已落下的方块和背景
            for (int i = 0; i < canvas_data.GetLength(0); i++)
            {
                for (int j = 0; j < canvas_data.GetLength(1); j++)
                {
                    //绘制方格
                    g.FillRectangle(Brushes.LightCyan, new Rectangle(j * cube_width, i * cube_width, cube_width - 2, cube_width - 2));
                    if (canvas_data[i, j] == 1)
                    {
                        // 数组下标（i，j）和Point（X，Y）的对应关系为X-》j，Y-》i
                        g.FillRectangle(fixedBrickColor, new Rectangle(j * cube_width, i * cube_width,cube_width-2,cube_width-2));
                    }
                }
            }
            //后绘制的像素会覆盖前一次绘制的像素位置
            //再绘制俄罗斯方块
            for ( int i =0;i < brick_width;i++)
            {
                for (int j = 0;j < brick_width; j++)
                {
                    if (brick_current.Data[i, j] == 1)
                    {
                        // 数组下标（i，j）和Point（X，Y）的对应关系为X-》j，Y-》i
                        int X = (j + brick_current.Location.X) * cube_width, Y = (i + brick_current.Location.Y) * cube_width;
                        g.FillRectangle(movingBrickColor, new Rectangle(
                            X,
                            Y,
                            cube_width-2,
                            cube_width-2));
                    }
                }
            }
            canvasBox.Image = canvas;
        }
        //满行后删除改行元素
        public void RemoveFilledRows() 
        {
            //记录需要消除的行
            List<int> rows = new List<int>();
            //按行扫描是否满行
            for (int i = 0; i < canvas_data.GetLength(0); i++)
            {
                bool isFilled = true;
                
                for(int j =0;j< canvas_data.GetLength(1); j++)
                {
                    if (canvas_data[i, j] == 0)
                    {
                        isFilled = false;
                    }
                }
                if (isFilled) rows.Add(i);
            }
            int needMoveRow = rows.Count;
            int[,] data = new int[canvas_row,canvas_column];
            for( int i = 0;i < canvas_data.GetLength (0); i++)
            {
                if (rows.Contains(i))
                {
                    needMoveRow--;
                    continue;
                }
                for(int j = 0;j < canvas_data.GetLength(1); j++)
                {
                    data[i+needMoveRow,j] = canvas_data[i,j];
                }
                
            }
            canvas_data = data;
        }
        //开始游戏
        public bool StartGame() 
        {
            //生成新方块
            if (GenNewBrick())
                return true;
            return false;
        }
        //游戏时钟Tick函数
        public bool Tick() 
        {   
            //删除满行
            RemoveFilledRows();
            //默认向下移动
            if (BrickMove(Brick.DIRECTION.DOWN))
            {
                Paint();
                return true;
            }
            //如果无法向下移动，则触底，生成新方块
            if (GenNewBrick())
            {
                Paint();
                return true;
            }

            return false;
        }
    }
}