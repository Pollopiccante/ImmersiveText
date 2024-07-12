using UnityEngine;

[CreateAssetMenu(fileName = "Chapter", menuName = "ScriptableObjects/ChapterScriptableObject", order = 1)]
public class ChapterScriptableObject : ScriptableObject
{
    public string chapterTitle;
    public string content;
}
