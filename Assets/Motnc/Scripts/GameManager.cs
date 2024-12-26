using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameObject seciliObje;
    GameObject seciliStand;
    Car _Car;
    public bool HareketVar;

    public int HedefStandSayisi;
    int TamamlananStandSayisi;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100))
            {
                if (hit.collider != null && hit.collider.CompareTag("Stand"))
                {
                    if (seciliObje != null && seciliStand != hit.collider.gameObject)
                    {
                        Stand _Stand = hit.collider.GetComponent<Stand>();
                        if (_Stand._Cars.Count != 4 && _Stand._Cars.Count != 0)
                        {
                            if (_Car.Renk == _Stand._Cars[^1].GetComponent<Car>().Renk)
                            {
                                seciliStand.GetComponent<Stand>().SoketDegistirmeIslemleri(seciliObje);
                                _Car.HareketEt("PozisyonDegistir", hit.collider.gameObject, _Stand.MusaitSoketiVer(), _Stand.HareketPozisyonu);
                                _Stand.bosOlanSoket++;
                                _Stand._Cars.Add(seciliObje);
                                _Stand.CarsKontrolEt();
                                seciliObje = null;
                                seciliStand = null;
                            }
                            else
                            {
                                _Car.HareketEt("SoketeGeriGit");
                                seciliObje = null;
                                seciliStand = null;
                            }


                        }
                        else if (_Stand._Cars.Count == 0)
                        {
                            seciliStand.GetComponent<Stand>().SoketDegistirmeIslemleri(seciliObje);
                            _Car.HareketEt("PozisyonDegistir", hit.collider.gameObject, _Stand.MusaitSoketiVer(), _Stand.HareketPozisyonu);
                            _Stand.bosOlanSoket++;
                            _Stand._Cars.Add(seciliObje);
                            _Stand.CarsKontrolEt();
                            seciliObje = null;
                            seciliStand = null;
                        }
                        else
                        {
                            _Car.HareketEt("SoketeGeriGit");
                            seciliObje = null;
                            seciliStand = null;
                        }
                    }
                    else if (seciliStand == hit.collider.gameObject)
                    {
                        _Car.HareketEt("SoketeGeriGit");
                        seciliObje = null;
                        seciliStand = null;
                    }
                    else
                    {
                        Stand _Stand = hit.collider.GetComponent<Stand>();
                        seciliObje = _Stand.EnUsttekiCarVer();
                        _Car = seciliObje.GetComponent<Car>();
                        HareketVar = true;

                        if (_Car.HareketEdebilirMi)
                        {
                            _Car.HareketEt("Secim", null, null, _Car._AitOlduguStand.GetComponent<Stand>().HareketPozisyonu);

                            seciliStand = _Car._AitOlduguStand;
                        }
                    }
                }
            }
        }
    }

    public void StandTamamlandi()
    {
        TamamlananStandSayisi++;
        if (TamamlananStandSayisi == HedefStandSayisi)
            Debug.Log("Kazandýn"); // Kazandýn paneli
    }
}
