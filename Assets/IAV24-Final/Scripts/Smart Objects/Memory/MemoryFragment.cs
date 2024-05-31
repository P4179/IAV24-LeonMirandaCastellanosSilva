using IAV24.Final;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "SmartObjects/Memory", fileName = "Memory")]
// un ScriptableObject funciona como un asset que sirve para
// almacenar datos
public class MemoryFragment : ScriptableObject
{
    private float elapsedTime = 0.0f;

    public int ocurrences { get; private set; } = 0;

    public string displayName;
    public float duration = 0.0f;   // duracion de este fragmento de memoria
    public ChangedStat[] changedStats;   // que estadisticas es a las que afecta
    public MemoryFragment[] memoryFragmentsThatCancelIt;   // fragmentos de memoria que lo cancelan

    // no se sobrecarga el operador == porque puede no funcionar en los scriptable objects
    public bool isTheSameAs(MemoryFragment other)
    {
        return name == other.name;
    }

    public bool isCancelledBy(MemoryFragment other)
    {
        foreach (var fragment in memoryFragmentsThatCancelIt)
        {
            if (fragment.isTheSameAs(other))
            {
                return true;
            }
        }

        return false;
    }

    public void newOcurrence(MemoryFragment other)
    {
        elapsedTime = Mathf.Max(elapsedTime, other.elapsedTime);
        ++ocurrences;
    }

    public MemoryFragment duplicate()
    {
        MemoryFragment newFragment = ScriptableObject.Instantiate(this);
        newFragment.name = name;
        newFragment.ocurrences = 1;
        newFragment.elapsedTime = 0.0f;
        return newFragment;
    }

    public bool updateTime()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= duration)
        {
            return true;
        }
        return false;
    }
}