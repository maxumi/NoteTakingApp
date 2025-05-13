using System;
using System.Collections.Generic;

namespace MauiCrossplatformApp.Models
{
    /// <summary>
    /// Visitor that can be applied to any node in the note‑folder tree.
    /// </summary>
    public interface INodeVisitor
    {
        void VisitFolder(Folder folder);
        void VisitNote(Note2 note);
    }

    /// <summary>
    /// Base abstraction for both folders and notes.
    /// </summary>
    public interface INode
    {
        Guid Id { get; }
        Folder? Parent { get; set; }
        string Name { get; }
        /// <summary>
        /// A *faux* path (pure string), built on demand – no real file system access.
        /// </summary>
        string Path { get; }

        void Accept(INodeVisitor visitor);
    }

    /// <summary>
    /// A folder can contain notes and other folders.
    /// </summary>
    public class Folder : INode
    {
        public Folder(string name = "")
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; set; }
        public Folder? Parent { get; set; }

        public List<INode> Children { get; } = new();

        // Faux path: root folder is "/"; otherwise parent + "/" + name.
        public string Path => Parent == null ? "/" : $"{Parent.Path.TrimEnd('/')}/{Name}";

        public void AddChild(INode node)
        {
            node.Parent = this;
            Children.Add(node);
        }

        public void Accept(INodeVisitor visitor)
        {
            visitor.VisitFolder(this);
            // depth‑first traversal
            foreach (var child in Children)
                child.Accept(visitor);
        }
    }

    /// <summary>
    /// A markdown note – leaf node in the tree.
    /// Renamed to Note2 for testing.
    /// </summary>
    public class Note2 : INode
    {
        public Note2(string fileName = "Untitled.md")
        {
            Id = Guid.NewGuid();
            Name = fileName;
            CreatedAt = UpdatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; }
        public string Name { get; set; }
        public Folder? Parent { get; set; }

        // Faux path: parent path + "/" + filename.
        public string Path => Parent == null ? Name : $"{Parent.Path.TrimEnd('/')}/{Name}";

        public string Content { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();

        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; private set; }

        public void Touch() => UpdatedAt = DateTime.UtcNow;

        public void Accept(INodeVisitor visitor) => visitor.VisitNote(this);
    }

    // ──────────────────────────────────────────
    // Example concrete visitor implementations
    // ──────────────────────────────────────────

    /// <summary>
    /// Collects every note that carries a specific tag.
    /// Usage:
    ///   var v = new TagCollectVisitor("todo");
    ///   root.Accept(v);
    ///   var matches = v.Notes;
    /// </summary>
    public class TagCollectVisitor : INodeVisitor
    {
        private readonly string _tag;
        public List<Note2> Notes { get; } = new();

        public TagCollectVisitor(string tag) => _tag = tag;

        public void VisitFolder(Folder folder) { /* nothing to do */ }

        public void VisitNote(Note2 note)
        {
            if (note.Tags.Contains(_tag))
                Notes.Add(note);
        }
    }

    /// <summary>
    /// Updates the modification timestamp of every visited note – could be used
    /// after a batch text replacement or migration.
    /// </summary>
    public class TouchAllNotesVisitor : INodeVisitor
    {
        public void VisitFolder(Folder folder) { /* recurse is done by Folder.Accept */ }

        public void VisitNote(Note2 note) => note.Touch();
    }
}
