using System;
using System.Collections.Generic;

public class Target
{
    public string Name;
    public Category Category;
    public Location Location;

    public Target(string name, Category category, Location location)
    {
        Name = name;
        Category = category;
        Location = location;
    }
}

public enum Category
{
    Item,
    Animal,
    Plant,
    Person,
}

