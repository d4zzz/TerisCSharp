using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Configuration;
using System.Linq;
using System.Text;

namespace Tetris
{
    public abstract class Brick
    {
        protected int[,] brick_data;
        protected Point location;

        // 移动方向
        public enum DIRECTION
        {
            LEFT = 1,
            RIGHT = 2,
            DOWN = 3,
        }
        // 方块类型
        public enum BRICK_TYPE
        {
            BRICK_BAR,
            BRICK_T,
            BRICK_L,
            BRICK_J,
            BRICK_Z,
            BRICK_SQURE
        }


        public Brick()
        {
            brick_data = new int[5,5];
            location = new Point();
        }
        
        public int[,] Data
        {
            get => brick_data;
            set
            {
            }
        }

        public Point Location
        {
            get => location;
            set
            {
                location = value;
            }
        }
        // 初始化俄罗斯方块，并进行碰撞检测
        public bool Init(int[,] canvas_data)
        {
            if(CollisionDetect(Location,Data,canvas_data))
                return false;
            return true;
        }

        public bool Move(DIRECTION direct, int[,] canvas_data)
        {
            switch (direct)
            {
                case DIRECTION.LEFT:
                    if (CollisionDetect(new Point(location.X - 1, location.Y), Data, canvas_data))
                        return false;
                    location.X -= 1;
                    break;
                case DIRECTION.RIGHT:
                    if (CollisionDetect(new Point(location.X + 1, location.Y), Data, canvas_data))
                        return false;
                    location.X += 1;
                    break;
            }
            return true;
        }

        public bool Rotate(int[,] canvas_data)
        {
            //检测是否是方阵
            if(brick_data.GetLength(0) != brick_data.GetLength(1))
            {
                return false;
            }
            //生成旋转后矩阵
            int[,] data = new int[brick_data.GetLength(0),brick_data.GetLength(1)];
            for(int i=0; i<data.GetLength(0); i++)
            {
                for(int j=0;j<data.GetLength(1); j++)
                {
                    data[i,j] = brick_data[brick_data.GetLength(0)-1-j,i];
                }
            }
            //进行碰撞检测
            if (CollisionDetect(Location, data, canvas_data))
            {
                return false;
            }
            brick_data = data;
            return true;
        }


        //传入顶点和画布数组进行碰撞检测
        public bool CollisionDetect(Point loc,int[,] brick,int[,] canvas)
        {

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    // 数组下标（i，j）和Point（X，Y）的对应关系为X-》j，Y-》i
                    if (brick[i,j] == 1 && ( 
                        loc.X + j < 0 ||                         //左边界
                        loc.X + j > canvas.GetLength(1) - 1||      //右边界
                        location.Y + i >= canvas.GetLength(0) - 1 ||      //下边界
                        canvas[loc.Y + i, loc.X + j] == 1  //和底部砖块碰撞
                        ))
                    {
                        return true;
                    }
                }
            }


            return false;
        }
    }
   
    public class BrickBar : Brick
    {
        public BrickBar() 
        { 
            brick_data = new int[, ] {
                 { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 1}, 
                { 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0},
                };
        }
    }

    public class BrickSqure : Brick
    {
        public BrickSqure() {
            brick_data = new int[,] {
                { 0, 0, 0, 0, 0 },
                { 0, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 0 },
                { 0, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0 },
                };
        }
    }

    public class BrickL : Brick
    {
        public BrickL() {
            brick_data = new int[,] {
                { 0, 0, 1, 0, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 0, 1, 1, 0 },
                { 0, 0, 0, 0, 0} };
        }
    }

    public class BrickT : Brick
    {
        public BrickT() {
            brick_data = new int[,] {
                { 0, 0, 0, 0, 0},
                { 0, 0, 1, 0, 0},
                { 0, 1, 1, 1, 0},
                { 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0},
                };
        }
    }

    public class BrickZ : Brick
    {
        public BrickZ() {
            brick_data = new int[,] {
                { 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0},
                { 0, 1, 1, 0, 0},
                { 0, 0, 1, 1, 0},
                { 0, 0, 0, 0, 0},
                
                 };
        }
    }

    public class BrickJ : Brick
    {
        public BrickJ() {
            brick_data = new int[,] {
                { 0, 0, 1, 0, 0},
                { 0, 0, 1, 0, 0},
                { 0, 0, 1, 0, 0},
                { 0, 1, 1, 0, 0},
                { 0, 0, 0, 0, 0} };
        }
    }
}