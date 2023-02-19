using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileManager
{
    public partial class FileManager : Form
    {
        public TreeNode rootNode;
        public BitMap bitmap = new BitMap();
        public Catalog rootCatalog = new Catalog("root");
        public Catalog curCatalog;
        private List<ListViewItem> listViewItems = new List<ListViewItem>();
        public string dir = Application.StartupPath;

        public FileManager()
        {
            InitializeComponent();
            FileStream f1, f2;
            if (!System.IO.Directory.Exists(System.IO.Path.Combine(@dir, "storage")))
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(@dir, "storage"));
            BinaryFormatter bf = new BinaryFormatter();
            if (System.IO.File.Exists(System.IO.Path.Combine(dir, "storage/rootCatalog.txt")) && System.IO.File.Exists(System.IO.Path.Combine(dir, "storage/bitMap.txt")))
            {
                f1 = new FileStream(System.IO.Path.Combine(dir, "storage/rootCatalog.txt"), FileMode.Open, FileAccess.Read, FileShare.Read);
                rootCatalog = bf.Deserialize(f1) as Catalog;
                f1.Close();

                f2 = new FileStream(System.IO.Path.Combine(dir, "storage/bitMap.txt"), FileMode.Open, FileAccess.Read, FileShare.Read);
                bitmap = bf.Deserialize(f2) as BitMap;
                f2.Close();
            }

            curCatalog = rootCatalog;
            textBox1.Text = trimPath(curCatalog.path);
            updateTreeView();
            updateListView();
        }

        private void FileManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            FileStream f1, f2;
            BinaryFormatter bf = new BinaryFormatter();
            f1 = new FileStream(System.IO.Path.Combine(dir, "storage/rootCatalog.txt"), FileMode.Create);
            bf.Serialize(f1, rootCatalog);
            f1.Close();
            f2 = new FileStream(System.IO.Path.Combine(dir, "storage/bitMap.txt"), FileMode.Create);
            bf.Serialize(f2, bitmap);
            f2.Close();

        }

        public string trimPath(string path)
        {
            string trimedPath = path;
            if (path.Length > 6)
            {
                string root = path.Substring(0, 5);
                string detail = path.Substring(5);
                trimedPath = root + detail;
            }

            return trimedPath;
        }
        //更新视图
        public void updateView()
        {
            updateTreeView();
            updateListView();
            textBox1.Text = trimPath(curCatalog.path);
        }

        //更新文件目录
        public void updateTreeView()
        {
            treeView1.Nodes.Clear();
            rootNode = new TreeNode("root");
            addTreeNode(rootNode, rootCatalog);
            treeView1.Nodes.Add(rootNode);
            rootNode.ExpandAll();
        }

        //更新视图目录
        public void updateListView()
        {
            listViewItems = new List<ListViewItem>();
            listView1.Items.Clear();
            if (curCatalog.nodelist != null)
            {
                for (int i = 0; i < curCatalog.nodelist.Count(); i++)
                {
                    ListViewItem file;
                    if (curCatalog.nodelist[i].nodeType == Node.NodeType.file)
                    {
                        file = new ListViewItem(new string[]{
                         curCatalog.nodelist[i].file.name+ ".txt",
                         curCatalog.nodelist[i].file.updatedTime.ToString(),
                         "文本文件",
                         curCatalog.nodelist[i].file.size.ToString()+" B"
                     });
                        file.ImageIndex = 0;
                    }
                    else
                    {
                        file = new ListViewItem(new string[]{
                         curCatalog.nodelist[i].folder.name ,
                         curCatalog.nodelist[i].folder.updatedTime.ToString(),
                         "文件夹",
                          "-"
                       });
                        file.ImageIndex = 1;
                    }
                    listViewItems.Add(file);
                    listView1.Items.Add(file);
                }
            }
            textBox2.Text ="  "+  listView1.Items.Count.ToString() + "个项目";
        }


        //递归增加子结点
        public void addTreeNode(TreeNode node, Catalog dir)
        {
            if (dir.nodelist != null)
            {
                for (int i = 0; i < dir.nodelist.Count(); i++)
                {
                    if (dir.nodelist[i].nodeType == Node.NodeType.folder)
                    {
                        TreeNode newNode = new TreeNode(dir.nodelist[i].name);
                        addTreeNode(newNode, dir.nodelist[i].folder);
                        node.Nodes.Add(newNode);
                    }
                }
            }
        }

        //视图项双击
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            if (listView1.SelectedItems.Count != 0)
            {
                item = listView1.SelectedItems[0];
                for (int i = 0; i < curCatalog.nodelist.Count(); i++)
                {
                    if (listViewItems[i] == item)
                    {
                        Node current_node = curCatalog.nodelist[i];
                        openListViewItem(ref current_node);
                        break;
                    }
                }
            }
         
        }

        //打开节点下视图
        private void openListViewItem(ref Node node)
        {
            if (node.nodeType == Node.NodeType.folder)
            {
                curCatalog = node.folder;
                textBox1.Text = trimPath(curCatalog.path);
                updateListView();
            }
            else
            {
                TxtEditor txtEditor = new TxtEditor(ref bitmap, ref node.file);
                txtEditor.Show();
                txtEditor.CallBack = updateView;
            }
             
        }


        //检测重名
        private String nameCheck(String name, String type)
        {
            int counter = 0;
            if (type == "")
            {
                for (int i = 0; i < curCatalog.nodelist.Count(); i++)
                {
                    if (curCatalog.nodelist[i].nodeType == Node.NodeType.folder)
                    {
                        string[] sArray = curCatalog.nodelist[i].folder.name.Split('(');
                        if (sArray[0] == name)
                        {
                            counter++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < curCatalog.nodelist.Count(); i++)
                {
                    if (curCatalog.nodelist[i].nodeType == Node.NodeType.file)
                    {
                        string[] sArray = curCatalog.nodelist[i].file.name.Split('(');
                        if (sArray[0] == name)
                        {
                            counter++;
                        }
                    }

                }
            }
            if (counter > 0)
                name += "(" + counter.ToString() + ")";
            return name;
        }

        //返回按钮
        private void 返回ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (curCatalog != rootCatalog)
            {
                curCatalog = curCatalog.parenCatalog;
                updateView();
            }
        }

        //打开按钮
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            if (listView1.SelectedItems.Count != 0)
            {
                item = listView1.SelectedItems[0];
                for (int i = 0; i < curCatalog.nodelist.Count(); i++)
                {
                    if (listViewItems[i] == item)
                    {
                        Node curNode = curCatalog.nodelist[i];
                        openListViewItem(ref curNode);
                        updataFolderSize(ref curCatalog);
                        break;
                    }
                }
            }
        }

        private void updataFolderSize(ref Catalog curCatalog)
        {
            curCatalog.fileSize = 0;
            for (int j = 0; j < curCatalog.nodelist.Count(); j++)
            {
                if (curCatalog.nodelist[j].nodeType == Node.NodeType.file)
                    curCatalog.fileSize += curCatalog.nodelist[j].file.size;
                else
                    curCatalog.fileSize += curCatalog.nodelist[j].folder.fileSize;
            }
        }

        private void 文件ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            String fileName = "新建文本文档";
            String fileType = "txt";
            fileName = nameCheck(fileName, fileType);
            FileCreator.operationType otype = FileCreator.operationType.newfile;
            FileCreator newfile = new FileCreator(ref curCatalog, fileName, fileType, otype);
            newfile.Show();
            newfile.CallBack = updateView;
          
        }

        private void 文件夹ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            String fileName = "新建文件夹";
            String fileType = "";
            fileName = nameCheck(fileName, fileType);
            FileCreator.operationType otype = FileCreator.operationType.newfile;
            FileCreator newfile = new FileCreator(ref curCatalog, fileName, fileType, otype);
            newfile.Show();
            newfile.CallBack = updateView;
           
        }

        private void 重命名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem current_item = new ListViewItem();
            String fileType = "txt";
            if (listView1.SelectedItems.Count != 0)
            {
                current_item = listView1.SelectedItems[0];
                for (int i = 0; i < curCatalog.nodelist.Count(); i += 1)
                {
                    if (listViewItems[i] == current_item)
                    {
                        if (curCatalog.nodelist[i].nodeType == Node.NodeType.folder)
                        {
                            fileType = "";
                        }
                        FileCreator.operationType op = FileCreator.operationType.rename;
                        FileCreator newfile = new FileCreator(ref curCatalog, curCatalog.nodelist[i].name, fileType, op);
                        newfile.Show();
                        newfile.CallBack = updateView;
                        break;
                    }
                }
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            if (listView1.SelectedItems.Count != 0)
            {
                item = listView1.SelectedItems[0];
                for (int i = 0; i < curCatalog.nodelist.Count(); i++)
                {
                    if (listViewItems[i] == item)
                    {
                        curCatalog.updatedTime = DateTime.Now;
                        delete(ref curCatalog.nodelist, i);
                        updataFolderSize(ref curCatalog);
                        updateView();
                        break;
                    }
                }
            }
        }

        public void delete(ref List<Node> nodelist, int i)
        {
            if (nodelist.Count() > 0)
            {
                if (nodelist[i].nodeType == Node.NodeType.file)
                {
                    nodelist[i].file.setEmpty(ref bitmap);
                    nodelist.RemoveAt(i);
                }
                else if (nodelist[i].nodeType == Node.NodeType.folder)
                {
                    if (nodelist[i].folder.nodelist != null)
                    {
                        for (int j = 0; j < nodelist[i].folder.nodelist.Count(); j++)
                        {
                            delete(ref nodelist[i].folder.nodelist, j);
                        }
                        nodelist.RemoveAt(i);
                    }
                    else
                    {
                        nodelist.RemoveAt(i);
                    }
                }
            }
            return;
        }

        private void 格式化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            delete(ref curCatalog.nodelist, 0);
            if (rootCatalog.nodelist.Count() != 0)
            {
                for (int i = 0; i < rootCatalog.nodelist.Count(); i++)
                {
                    delete(ref rootCatalog.nodelist, i);
                }
            }
            rootCatalog = new Catalog("root");
            curCatalog = rootCatalog;
            updateView();
        }

        private void 属性ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                ListViewItem item = new ListViewItem();
                item = listView1.SelectedItems[0];
                for (int i = 0; i < curCatalog.nodelist.Count(); i++)
                {
                    if (listViewItems[i] == item)
                    {
                        FileAttributes attributes = new FileAttributes(curCatalog.nodelist[i]);
                        attributes.Show();
                    }
                }
            }
            else
            {
                if (curCatalog.parenCatalog != null)
                {
                    for (int i = 0; i < curCatalog.parenCatalog.nodelist.Count(); i++)
                    {
                        if (curCatalog.parenCatalog.nodelist[i].name == curCatalog.name)
                        {
                            FileAttributes attributes = new FileAttributes(curCatalog.parenCatalog.nodelist[i]);
                            attributes.Show();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("根目录");
                }
            }
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateView();
        }
    }

    [Serializable]
    public class File
    {
        public const int TXT = 0;

        public int type;   //类型
        public int size;  // 大小
        public String name;  // 文件名
        public DateTime createdTime;  // 创建时间
        public DateTime updatedTime;  // 修改时间
        public List<Block> blocklist;  // 文件指针
        public String path;

        public File(String name, String type, String fatherPath)
        {
            this.type = TXT;
            this.name = name;
            createdTime = DateTime.Now;
            updatedTime = DateTime.Now;
            size = 0;
            blocklist = new List<Block>();
            path = fatherPath + "\\" + name;
        }

        public void setEmpty(ref BitMap bitmap)
        {
            for (int i = 0; i < blocklist.Count(); i += 1)
            {
                bitmap.setFree(bitmap.findFreeBlock());
            }
            blocklist.Clear();
            size = 0;
        }

        public void writeFile(String data, ref BitMap bitmap)
        {
            setEmpty(ref bitmap);
            while (data.Count() > 512)
            {
                bitmap.blocks[bitmap.findFreeBlock()] = new Block();
                bitmap.blocks[bitmap.findFreeBlock()].setData(data.Substring(0, 512));   
                blocklist.Add(bitmap.blocks[bitmap.findFreeBlock()]);   
                bitmap.setOccupy(bitmap.findFreeBlock());   
                size += 512;
                data = data.Remove(0, 512);
            }
            bitmap.blocks[bitmap.findFreeBlock()] = new Block();
            bitmap.blocks[bitmap.findFreeBlock()].setData(data);
            blocklist.Add(bitmap.blocks[bitmap.findFreeBlock()]);  
            bitmap.setOccupy(bitmap.findFreeBlock());
            size += data.Length;
            updatedTime = DateTime.Now;
        }

        public String getFileContent()
        {
            string content = "";
            for (int i = 0; i < blocklist.Count(); i += 1)
            {
                content += blocklist[i].getData();
            }
            return content;
        }
   

    }
    [Serializable]
    public class BitMap
    {
        public const int BYTE_SIZE = 8;
        public const int MAX_CAPCITY = 100 * 100;
        public const int BYTENUMBER = 100 * 100 / 8;
        public Block[] blocks = new Block[MAX_CAPCITY];
        public bool[] bitMap = new bool[MAX_CAPCITY]; 

        public BitMap()
        {
            for (int i = 0; i < MAX_CAPCITY; i++)
            {
                bitMap[i] = true;
            }
        }

        public int findFreeBlock()
        {
            int bytePos = 0, bitPos = 0; 
            while (bytePos < BYTENUMBER) 
            {
                if (bitMap[bytePos * BYTE_SIZE + bitPos])
                {
                    return (bytePos * BYTE_SIZE + bitPos);
                }
                else
                {
                    bitPos += 1;
                    if (bitPos == BYTE_SIZE)
                    {
                        bitPos = bitPos % BYTE_SIZE;
                        bytePos += 1;
                    }
                }
            }
            return -1;
        }

        public void setFree(int i)
        {
            bitMap[i] = true;
        }

        public void setOccupy(int i)
        {
            bitMap[i] = false;
        }
    }
    [Serializable]
    public class Block
    {
        public const int BLOCKSIZE = 512; 
        public char[] data; 
        public int length;  

        public Block()
        {
            data = new char[BLOCKSIZE];
        }

        public void setData(String newData)
        {
            length = (newData.Length > 512) ? 512 : newData.Length;
            for (int i = 0; i < length; i++)
            {
                data[i] = newData[i];
            }
        }

        public String getData()
        {
            String temp = new String(data);
            return temp;
        }
    }
    [Serializable]
    public class Catalog
    {
        public List<Node> nodelist; 
        public int childrenNum; 
        public String name; 
        public String path;
        public int fileSize; 
        public DateTime createdTime; 
        public DateTime updatedTime; 
        public Catalog parenCatalog = null; 

        
        public Catalog(String namedata, String fatherPath)
        {
            nodelist = new List<Node>();
            name = namedata;
            path = fatherPath + '\\' + namedata;
            createdTime = DateTime.Now;
            updatedTime = DateTime.Now;
            fileSize = 0;
            childrenNum = 0;
        }

       
        public Catalog(String namedata)
        {
            nodelist = new List<Node>();
            name = namedata;
            path = namedata + ":";
            createdTime = DateTime.Now;
            updatedTime = DateTime.Now;
            fileSize = 0;
            childrenNum = 0;
        }

        
        public void addNode(Catalog catalog, String namedata, String fatherPath)
        {
            Node node = new Node(namedata, fatherPath);
            node.folder.parenCatalog = catalog;
            nodelist.Add(node);
            childrenNum += 1;
            updatedTime = DateTime.Now;
        }

       
        public void addNode(String namedata, String fileType, String fatherPath)
        {
            Node node = new Node(namedata, fileType, fatherPath);
            nodelist.Add(node);
            childrenNum += 1;
            updatedTime = DateTime.Now;
        }
    }
    [Serializable]
    public class Node
    {
        public enum NodeType { folder, file }; 
        public NodeType nodeType;
        public File file;     
        public Catalog folder; 
        public String path;
        public String name;

        public Node(String namedata, String fatherPath)   //文件夹结点
        {
            nodeType = NodeType.folder;
            path = fatherPath + "\\" + namedata;
            name = namedata;
            folder = new Catalog(namedata, fatherPath);
        }

        public Node(String namedata, String fileType, String fatherPath)    //文件结点
        {
            nodeType = NodeType.file;
            path = fatherPath + '\\' + namedata;
            name = namedata;
            file = new File(name, fileType, fatherPath);
        }

        public void reName(String newName)
        {
            name = newName;
            if (nodeType == Node.NodeType.folder)
            {
                folder.path = folder.path.Remove(folder.path.Length - folder.name.Length - 1, folder.name.Length + 1);
                folder.name = newName;
                folder.path = folder.path + "\\" + folder.name;
            }
            else
            {
                file.path = file.path.Remove(file.path.Length - file.name.Length - 1, file.name.Length + 1);
                file.name = newName;
                file.path = file.path + "\\" + file.name;
            }
        }
    }
}
