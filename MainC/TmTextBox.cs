using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MainC
{
    public class MyTextBox : TextBox
    {
        const int WM_ERASEBKGND = 0x0014;

        private Image backImage;

        [DisplayName("背景图片")]
        public Image BackImage
        {
            get { return backImage; }
            set { backImage = value; }
        }

        protected void OnEraseBkgnd(Graphics gs)
        {
            gs.FillRectangle(Brushes.White, 0, 0, this.Width, this.Height); //填充为白色，防止图片太小出现重影  
            if (backImage != null) gs.DrawImage(backImage, 0, 0); //绘制背景。  
            gs.Dispose();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ERASEBKGND) //绘制背景  
            {
                OnEraseBkgnd(Graphics.FromHdc(m.WParam));
                m.Result = (IntPtr)1;
            }
            base.WndProc(ref m);
        }
    }
}
