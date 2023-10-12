using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textcontroller : MonoBehaviour
{
    public TextMeshProUGUI infoVamp1;
    public TextMeshProUGUI infoVamp2;
    public TextMeshProUGUI infoBat;
    // Start is called before the first frame update
    void Start()
    {
        infoVamp1.text = "You're about to encounter a basic Jiangshi. The plague has rotted its eyes out. It can't see, but it can detect anything breathing. Avoid getting detected by <u>holding your breath</u>.";
        infoVamp2.text = "The feathered Jiangshi pairs with a Monobat that enables it to 'see'. Holding your breath while the bat is active does not good. This Jiangshi acts as a basic Jiangshi when the bat is either blinded or vanquished.";
        infoBat.text = "The Monobat is a plague-mutated mono-eyed creature. Once it spots you, it persistently keeps it's eye on you. You can <u>momentarily blind it with a bright flash of light and jump on it</u> to vanquish it.";   }

}
