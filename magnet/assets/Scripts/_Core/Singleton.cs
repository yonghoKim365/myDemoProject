using System;
using System.Collections;

public class Singleton<TManager, TBase>
{
    static Singleton()
    {
        if (instance == null)
        {
            instance = (TManager)Activator.CreateInstance(typeof(TManager), true);
            hashTable = new Hashtable();
        }
    }

    public static TManager instance { get; private set; }
    public static Hashtable hashTable { get; private set; }

    public void Add(int paramUID, TBase paramBase)
    {
        hashTable.Add(paramUID, paramBase);
    }

    public TBase Find(int paramUID)
    {
        return (TBase)hashTable[paramUID];
    }
	
	public void Remove(int paramUID)
	{
		if(hashTable.Contains(paramUID))
			hashTable.Remove(paramUID);
	}
	
	public void RemoveAll()
	{
		hashTable.Clear();
	}
	
    public virtual void Update()
    {
    }
}

public class Singleton<TManager>
{
    static Singleton()
    {
        if (instance == null)
            instance = (TManager)Activator.CreateInstance(typeof(TManager), true);
    }

    public static TManager instance { get; set; }

    public virtual void Update()
    {
    }
}