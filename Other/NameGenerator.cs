using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NameGenerator
{
    private static string[] BaseName = new string[] { "Василий", "Ким", "Олжас", "Денис", "Иван", "Владимир", "Цезарь", "Джо", "Крендель", "ПК" };
    private static string[] BaseSurname = new string[] { "Кот", "Борисов", "Хитрый", "Ширяев", "Иванов", "Смирнов", "Усатый", "Непоедимый", "Быстрый", "Лучше" };

    public static string NewName()
    {
        return BaseName[Random.Range(0, BaseName.Length)];
    }

    public static string NewSurname()
    {
        return BaseSurname[Random.Range(0, BaseSurname.Length)];
    }
}