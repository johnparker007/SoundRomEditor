﻿
namespace SoundRomEditor
{
    partial class ViewMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonPlayAll = new System.Windows.Forms.Button();
            this.ButtonSaveAllWavs = new System.Windows.Forms.Button();
            this.ButtonLoadROMs = new System.Windows.Forms.Button();
            this.SamplesDataGridView = new System.Windows.Forms.DataGridView();
            this.ButtonLoadWav = new System.Windows.Forms.Button();
            this.ButtonSaveROMs = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SamplesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonPlayAll
            // 
            this.ButtonPlayAll.Location = new System.Drawing.Point(12, 70);
            this.ButtonPlayAll.Name = "ButtonPlayAll";
            this.ButtonPlayAll.Size = new System.Drawing.Size(118, 23);
            this.ButtonPlayAll.TabIndex = 0;
            this.ButtonPlayAll.Text = "Play All";
            this.ButtonPlayAll.UseVisualStyleBackColor = true;
            this.ButtonPlayAll.Click += new System.EventHandler(this.OnButtonPlayAllClick);
            // 
            // ButtonSaveAllWavs
            // 
            this.ButtonSaveAllWavs.Location = new System.Drawing.Point(12, 99);
            this.ButtonSaveAllWavs.Name = "ButtonSaveAllWavs";
            this.ButtonSaveAllWavs.Size = new System.Drawing.Size(118, 23);
            this.ButtonSaveAllWavs.TabIndex = 1;
            this.ButtonSaveAllWavs.Text = "Save All Wavs";
            this.ButtonSaveAllWavs.UseVisualStyleBackColor = true;
            this.ButtonSaveAllWavs.Click += new System.EventHandler(this.OnButtonSaveAllWavsClick);
            // 
            // ButtonLoadROMs
            // 
            this.ButtonLoadROMs.Location = new System.Drawing.Point(12, 12);
            this.ButtonLoadROMs.Name = "ButtonLoadROMs";
            this.ButtonLoadROMs.Size = new System.Drawing.Size(118, 23);
            this.ButtonLoadROMs.TabIndex = 2;
            this.ButtonLoadROMs.Text = "Load ROMs";
            this.ButtonLoadROMs.UseVisualStyleBackColor = true;
            this.ButtonLoadROMs.Click += new System.EventHandler(this.OnButtonLoadRomsClick);
            // 
            // SamplesDataGridView
            // 
            this.SamplesDataGridView.AllowUserToAddRows = false;
            this.SamplesDataGridView.AllowUserToDeleteRows = false;
            this.SamplesDataGridView.AllowUserToResizeColumns = false;
            this.SamplesDataGridView.AllowUserToResizeRows = false;
            this.SamplesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.SamplesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SamplesDataGridView.Location = new System.Drawing.Point(136, 12);
            this.SamplesDataGridView.MultiSelect = false;
            this.SamplesDataGridView.Name = "SamplesDataGridView";
            this.SamplesDataGridView.ReadOnly = true;
            this.SamplesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.SamplesDataGridView.ShowEditingIcon = false;
            this.SamplesDataGridView.Size = new System.Drawing.Size(435, 657);
            this.SamplesDataGridView.TabIndex = 3;
            this.SamplesDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnSamplesDataGridViewCellClick);
            this.SamplesDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnSamplesDataGridViewCellContentClick);
            // 
            // ButtonLoadWav
            // 
            this.ButtonLoadWav.Location = new System.Drawing.Point(577, 12);
            this.ButtonLoadWav.Name = "ButtonLoadWav";
            this.ButtonLoadWav.Size = new System.Drawing.Size(118, 23);
            this.ButtonLoadWav.TabIndex = 4;
            this.ButtonLoadWav.Text = "Load WAV";
            this.ButtonLoadWav.UseVisualStyleBackColor = true;
            this.ButtonLoadWav.Click += new System.EventHandler(this.OnButtonLoadWavClick);
            // 
            // ButtonSaveROMs
            // 
            this.ButtonSaveROMs.Location = new System.Drawing.Point(12, 41);
            this.ButtonSaveROMs.Name = "ButtonSaveROMs";
            this.ButtonSaveROMs.Size = new System.Drawing.Size(118, 23);
            this.ButtonSaveROMs.TabIndex = 5;
            this.ButtonSaveROMs.Text = "Save ROMs";
            this.ButtonSaveROMs.UseVisualStyleBackColor = true;
            this.ButtonSaveROMs.Click += new System.EventHandler(this.OnButtonSaveRomsClick);
            // 
            // ViewMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.ButtonSaveROMs);
            this.Controls.Add(this.ButtonLoadWav);
            this.Controls.Add(this.SamplesDataGridView);
            this.Controls.Add(this.ButtonLoadROMs);
            this.Controls.Add(this.ButtonSaveAllWavs);
            this.Controls.Add(this.ButtonPlayAll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ViewMainForm";
            this.Text = "Sound ROM Editor";
            this.Load += new System.EventHandler(this.OnViewMainFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.SamplesDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private void SamplesDataGridView_DataError1(object sender, System.Windows.Forms.DataGridViewDataErrorEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.Button ButtonPlayAll;
        private System.Windows.Forms.Button ButtonSaveAllWavs;
        private System.Windows.Forms.Button ButtonLoadROMs;
        private System.Windows.Forms.DataGridView SamplesDataGridView;
        private System.Windows.Forms.Button ButtonLoadWav;
        private System.Windows.Forms.Button ButtonSaveROMs;
    }
}

