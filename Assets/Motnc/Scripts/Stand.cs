using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stand : MonoBehaviour
{
    public GameObject HareketPozisyonu;
    public GameObject[] Soketler;
    public int bosOlanSoket;
    public List<GameObject> _Cars = new();
    [SerializeField] private GameManager _GameManager;

    int CarTamamlanmaSayisi;
    
    public GameObject EnUsttekiCarVer()
    {
        return _Cars[^1]; //_Cars.Count-1; == ^1
    }

    public GameObject MusaitSoketiVer()
    {
        return Soketler[bosOlanSoket];
    }

    public void SoketDegistirmeIslemleri(GameObject SilinecekObje)
    {
        _Cars.Remove(SilinecekObje);
        if (_Cars.Count != 0)
        {
            bosOlanSoket--;
            _Cars[^1].GetComponent<Car>().HareketEdebilirMi = true;
        }
        else
        {
            bosOlanSoket = 0;
        }
    }

    public void CarsKontrolEt()
    {
        if (_Cars.Count == 3)
        {
            string Renk= _Cars[0].GetComponent<Car>().Renk;

            foreach (var item in _Cars)
            {
                if (Renk == item.GetComponent<Car>().Renk)
                    CarTamamlanmaSayisi++;
            }

            if (CarTamamlanmaSayisi == 3)
            {
                _GameManager.StandTamamlandi();
                TamamlanmisStandIslemleri();
            }
            else
            {
                CarTamamlanmaSayisi = 0;
            }
        }
    }

    void TamamlanmisStandIslemleri()
    {
        foreach (var item in _Cars)
        {
            item.GetComponent<Car>().HareketEdebilirMi = false;

            Color32 color = item.GetComponent<MeshRenderer>().material.GetColor("_Color");
            color.a = 150;
            item.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
            gameObject.tag = "TamamlanmisStand";
        }
    }
}
