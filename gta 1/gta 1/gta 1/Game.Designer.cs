namespace gta_1
{
    partial class Game
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.TimerGameLoop = new System.Windows.Forms.Timer(this.components);
            this.Screen = new gta_1.DoubleBufferedPanel();
            this.SuspendLayout();
            // 
            // TimerGameLoop
            // 
            this.TimerGameLoop.Interval = 20;
            this.TimerGameLoop.Tick += new System.EventHandler(this.TimerGameLoop_Tick);
            // 
            // Screen
            // 
            this.Screen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Screen.Location = new System.Drawing.Point(0, 0);
            this.Screen.Name = "Screen";
            this.Screen.Size = new System.Drawing.Size(1264, 681);
            this.Screen.TabIndex = 0;
            this.Screen.Paint += new System.Windows.Forms.PaintEventHandler(this.Screen_Paint);
            this.Screen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Screen_MouseClick);
            this.Screen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Game_MouseMove);
            // 
            // Game
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.Screen);
            this.KeyPreview = true;
            this.Name = "Game";
            this.Text = "gta 1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Game_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Game_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private gta_1.DoubleBufferedPanel Screen;
        private System.Windows.Forms.Timer TimerGameLoop;
    }
}

