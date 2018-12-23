﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaSTGEditorSharp.EditorData;
using LuaSTGEditorSharp.EditorData.Document;
using LuaSTGEditorSharp.EditorData.Document.Meta;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;

namespace LuaSTGEditorSharp.EditorData.Node.Graphics
{
    [Serializable, NodeIcon("/LuaSTGNodeLib;component/images/16x16/loadimage.png")]
    [ClassNode]
    [LeafNode]
    [CreateInvoke(0), RCInvoke(4)]
    public class LoadImage : TreeNode
    {
        [JsonConstructor]
        private LoadImage() : base() { }

        public LoadImage(DocumentData workSpaceData)
            : this(workSpaceData, "", "", "true", "0,0", "false", "0") { }

        public LoadImage(DocumentData workSpaceData, string path, string name, string mipmap, string collis, string rect, string edge)
            : base(workSpaceData)
        {
            attributes.Add(new DependencyAttrItem("Path", path, this, "imageFile"));
            attributes.Add(new AttrItem("Resource name", name, this));
            attributes.Add(new AttrItem("Mipmap", mipmap, this, "bool"));
            attributes.Add(new AttrItem("Collision size", collis, this, "size"));
            attributes.Add(new AttrItem("Rectangle collision", rect, this, "bool"));
            attributes.Add(new AttrItem("Cut edge", edge, this));
        }

        public override IEnumerable<string> ToLua(int spacing)
        {
            string sp = "".PadLeft(spacing * 4);
            yield return sp + "_LoadImageFromFile(\'image:\'..\'" + Lua.StringParser.ParseLua(NonMacrolize(1))
                + "\',\'" + Lua.StringParser.ParseLua(Path.GetFileName(NonMacrolize(0)))
                + "\'," + Macrolize(2) + "," + Macrolize(3) + "," + Macrolize(4) + "," + Macrolize(5) + ")\n";
        }

        public override IEnumerable<Tuple<int,TreeNode>> GetLines()
        {
            yield return new Tuple<int, TreeNode>(1, this);
        }

        public override string ToString()
        {
            return "Load image \"" + NonMacrolize(1) + "\" from \"" + NonMacrolize(0) + "\"";
        }

        public override void ReflectAttr(DependencyAttrItem relatedAttrItem, string originalvalue)
        {
            if (relatedAttrItem.AttrInput != originalvalue) 
            {
                attributes[1].AttrInput = Path.GetFileNameWithoutExtension(attributes[0].AttrInput);
            }
        }

        protected override void AddCompileSettings()
        {
            if (!parentWorkSpace.CompileProcess.resourceFilePath.Contains(NonMacrolize(0)))
            {
                parentWorkSpace.CompileProcess.resourceFilePath.Add(attributes[0].AttrInput);
            }
        }

        public override MetaInfo GetMeta()
        {
            return new ImageLoadMetaInfo(this);
        }

        public override object Clone()
        {
            var n = new LoadImage(parentWorkSpace)
            {
                attributes = new ObservableCollection<AttrItem>(from AttrItem a in attributes select (AttrItem)a.Clone()),
                Children = new ObservableCollection<TreeNode>(from TreeNode t in Children select (TreeNode)t.Clone()),
                _parent = _parent,
                isExpanded = isExpanded
            };
            n.FixAttrParent();
            n.FixChildrenParent();
            return n;
        }
    }
}