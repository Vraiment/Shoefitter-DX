﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ShoefitterDX.Panes
{
    public class GameExplorerPane : DockContent
    {
        private class TreeEntryMeta
        {
            public string Path { get; }
            public bool IsDirectory { get; }

            public TreeEntryMeta(string path, bool isDirectory)
            {
                this.Path = path;
                this.IsDirectory = isDirectory;
            }
        }

        public string SelectedAsset
        {
            get
            {
                TreeEntryMeta meta = treeView1.SelectedNode?.Tag as TreeEntryMeta;
                if (meta != null && !meta.IsDirectory)
                {
                    return meta.Path;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (String.IsNullOrEmpty(value) && !String.IsNullOrEmpty(SelectedAsset))
                {
                    treeView1.SelectedNode = null;
                }
                else
                {
                    if (FileNodes.ContainsKey(value))
                    {
                        treeView1.SelectedNode = FileNodes[value];
                    }
                    else
                    {
                        Program.Window.Output.WriteText("[WARNING]: GameExplorerPane couldn't select path '" + value + "'!");
                        Program.Window.Output.WriteText("           That isn't a valid relative path in the project.");
                    }
                }
            }
        }

        private TreeNode ProjectNode;
        private Dictionary<string, TreeNode> FileNodes = new Dictionary<string, TreeNode>(StringComparer.OrdinalIgnoreCase);

        private System.Windows.Forms.TreeView treeView1;

        public GameExplorerPane()
        {
            InitializeComponent();

            // Dock content options
            this.DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom;

            // Connect to project events
            Program.ProjectOpened += (_, p) => RefreshProject();
            Program.ProjectClosed += (_, p) => RefreshProject();

            // Forward pane focus to the TreeView
            this.GotFocus += GameExplorerPane_GotFocus;

            // Create tree nodes
            ProjectNode = new TreeNode("<ProjectNode>");

            treeView1.Nodes.Add(ProjectNode);

            RefreshProject();
        }

        private void GameExplorerPane_GotFocus(object sender, EventArgs e)
        {
            treeView1.Focus();
        }

        public bool AssetExists(string filename)
        {
            return FileNodes.ContainsKey(filename);
        }

        private void AddFileNode(string filename, TreeNode directoryNode)
        {
            TreeNode newNode = new TreeNode(System.IO.Path.GetFileName(filename));
            string relativePath = Program.Project.MakePathRelative(filename);
            newNode.Tag = new TreeEntryMeta(relativePath, false);
            FileNodes.Add(relativePath, newNode);
            directoryNode.Nodes.Add(newNode);
        }

        private void AddDirectoryNodes(string directory, TreeNode directoryNode)
        {
            foreach (string childDirectory in System.IO.Directory.EnumerateDirectories(directory))
            {
                if (System.IO.Path.GetFileName(childDirectory).StartsWith("."))
                    continue;
                TreeNode newNode = new TreeNode(System.IO.Path.GetFileName(childDirectory));
                newNode.Tag = new TreeEntryMeta(childDirectory, true);
                AddDirectoryNodes(childDirectory, newNode);
                directoryNode.Nodes.Add(newNode);
            }

            foreach (string filename in System.IO.Directory.EnumerateFiles(directory))
            {
                if (System.IO.Path.GetFileName(filename).StartsWith("."))
                    continue;
                AddFileNode(filename, directoryNode);
            }
        }

        private void RefreshProject()
        {
            FileNodes.Clear();
            ProjectNode.Nodes.Clear();
            if (Program.Project == null)
            {
                ProjectNode.Text = "<ProjectNode>";
                if (treeView1.Nodes.Contains(ProjectNode))
                    treeView1.Nodes.Remove(ProjectNode);
                Text = "Game Explorer - No Project";
                ProjectNode.Tag = new TreeEntryMeta("", true); // A dummy that will be replaced on load
            }
            else
            {
                ProjectNode.Text = System.IO.Path.GetFileNameWithoutExtension(Program.Project.Filename);
                if (!treeView1.Nodes.Contains(ProjectNode))
                    treeView1.Nodes.Add(ProjectNode);
                Text = "Game Explorer - " + ProjectNode.Text;
                treeView1.BeginUpdate();
                AddDirectoryNodes(System.IO.Path.GetDirectoryName(Program.Project.Filename), ProjectNode);
                ProjectNode.Tag = new TreeEntryMeta(System.IO.Path.GetDirectoryName(Program.Project.Filename), true);
                treeView1.EndUpdate();
            }
        }

        private void InitializeComponent()
        {
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowLines = false;
            this.treeView1.Size = new System.Drawing.Size(284, 261);
            this.treeView1.TabIndex = 0;
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.TreeView1_ItemDrag);
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView1_NodeMouseDoubleClick);
            // 
            // GameExplorerPane
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.treeView1);
            this.Name = "GameExplorerPane";
            this.Text = "Game Explorer";
            this.ResumeLayout(false);

        }

        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeEntryMeta meta = e?.Node?.Tag as TreeEntryMeta;
            if (meta != null)
            {
                if (!meta.IsDirectory)
                {
                    Program.Window.OpenFileEditor(meta.Path);
                }
            }
        }

        private void TreeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeEntryMeta nodeMeta = ((e.Item as TreeNode)?.Tag as TreeEntryMeta);
            if (nodeMeta != null && !nodeMeta.IsDirectory)
            {
                DoDragDrop(nodeMeta.Path, DragDropEffects.Link);
            }
        }
    }
}
