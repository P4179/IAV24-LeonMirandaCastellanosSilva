using IAV24.Final;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    [SerializeField]
    private GameObject magicPowerPanel;

    HashSet<Performer> participants = new HashSet<Performer>();
    int nClosedBooks = 0;

    public static BookManager Instance { get; private set; } = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        magicPowerPanel.SetActive(false);
    }

    public void addClosedBook()
    {
        ++nClosedBooks;
    }

    public void registerOpenedBook(Performer performer)
    {
        if (!participants.Contains(performer))
        {
            participants.Add(performer);
        }

        --nClosedBooks;
        if (nClosedBooks <= 0)
        {
            // se activa el poder magico de todos los usuarios que hayan participado
            // en la apertura de libros
            foreach (Performer participant in participants)
            {
                MagicPower magicPower = participant.GetComponentInChildren<MagicPower>();
                if (magicPower != null)
                {
                    magicPower.enabled = true;
                }
            }
            magicPowerPanel.SetActive(true);
        }
    }

    public bool allBooksOpened()
    {
        return nClosedBooks <= 0;
    }
}
