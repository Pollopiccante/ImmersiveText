
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class TextManager : EditorWindow
{

    [MenuItem("Window/TextManager")]
    public static void ShowWindow()
    {
        GetWindow<TextManager>("TextManager");
    }

    private Tuple<string, string>[] SplitIntoChapters(string book, string chapterDelimiterRegex)
    {
        // split chapter content
        string[] chapterContent = Regex.Split(book, chapterDelimiterRegex);

        // get matched chapter titles
        LinkedList<string> chapterTitlesList = new LinkedList<string>(); 
        foreach (Match m in Regex.Matches(book, chapterDelimiterRegex))
        {
            chapterTitlesList.AddLast(m.Value.Trim('\n').Replace("\n", ", "));
        }
        string[] chapterTitles = chapterTitlesList.ToArray();
        
        // check for smaller number
        LinkedList<Tuple<string, string>> output = new LinkedList<Tuple<string, string>>();
        for (int i = 0; i < Mathf.Min(chapterContent.Length, chapterTitles.Length); i++)
        {
            output.AddLast(new Tuple<string, string>(chapterTitles[i], chapterContent[i]));
        }
        return output.ToArray();
    }


    // input
    private Object _source;
    private string _title;
    private string _author;
    private string _chapterDelimiterRegex = @"CHAPTER (\d+)\n(.*)\n";

    // preview
    private Tuple<string, string>[] _chapters;
    
    // Ui variables
    private Vector2 _scrollPosition;
    private string _warnings;
    
    // warnings
    private string _noSource = "Please specify a text source file. ";
    private string _noRegex = "Please specify a regex. ";
    private string _noTitle = "Please specify a title. ";
    private string _noAuthor = "Please specify an author. ";

    private void OnGUI()
    {
        GUILayout.Label("Text to Scriptable Object Conversion");
        GUILayout.BeginVertical();
        
        _source = EditorGUILayout.ObjectField("Text File", _source, typeof(Object), true);
        TextAsset newTxtAsset = (TextAsset)_source;

        _title = EditorGUILayout.TextField("Title", _title);
        _author = EditorGUILayout.TextField("Author", _author);
        _chapterDelimiterRegex = EditorGUILayout.TextField("Chapter Delimiter Regex", _chapterDelimiterRegex);

        if (GUILayout.Button("Preview"))
        {
            _warnings = "";
            if (String.IsNullOrEmpty(_chapterDelimiterRegex))
                _warnings += _noRegex;
            if (_source == null)
                _warnings += _noSource;
            
            if (String.IsNullOrEmpty(_warnings))
                _chapters = SplitIntoChapters(newTxtAsset.text, _chapterDelimiterRegex);
        }

        if (GUILayout.Button("Generate Scriptable Objects"))
        {
            _warnings = "";
            if (String.IsNullOrEmpty(_title))
                _warnings += _noTitle;
            if (String.IsNullOrEmpty(_chapterDelimiterRegex))
                _warnings += _noRegex;
            if (String.IsNullOrEmpty(_author))
                _warnings += _noAuthor;
            if (_source == null)
                _warnings += _noSource;

            if (String.IsNullOrEmpty(_warnings))
            {
                // create book scriptable object
                BookScriptableObject book = CreateInstance<BookScriptableObject>();
                book.title = _title;    
                book.author = _author;
                AssetDatabase.CreateFolder("Assets/TextResources", book.title);

                // save book scriptable object
                AssetDatabase.CreateAsset(book, "Assets/TextResources/" + book.title + "/book.asset");
            
                // generate chapters if they are not present
                if (_chapters.Length == 0)
                {
                    _chapters = SplitIntoChapters(newTxtAsset.text, _chapterDelimiterRegex);
                }
            
                // create, save and add chapter scriptable objects to the book scriptable object
                book.chapters = new ChapterScriptableObject[_chapters.Length];
                for (int i=0; i<_chapters.Length; i++)
                {
                    // create
                    ChapterScriptableObject chapter = CreateInstance<ChapterScriptableObject>();
                    chapter.chapterTitle = _chapters[i].Item1;
                    chapter.content = _chapters[i].Item2;
                    // save
                    AssetDatabase.CreateAsset(chapter, "Assets/TextResources/" + book.title + "/chapter" + i + ".asset");
                    // add
                    book.chapters[i] = chapter;
                }
            }
        }
        
        // warning
        if (!String.IsNullOrEmpty(_warnings))
        {
            EditorGUILayout.HelpBox(new GUIContent(_warnings));
        }
        
        // preview
        if (_chapters != null && _chapters.Length > 0)
        {
            string message = _chapters.Length + " chapters found:\n";
            foreach (var chapter in _chapters)
            {
                message += chapter.Item1 + "\n";
            }
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            EditorGUILayout.HelpBox(new GUIContent(message));
            GUILayout.EndScrollView();
        }
        
        GUILayout.EndVertical();
    }
}
