using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 환형 Queue 클래스.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CircularQueue<T> where T : class, new()
{
    #region VALUES

    private const int MALLOC_SIZE = 1;

    private List<T> List = null;

    #endregion

    #region PROPERTY

    public int Capacity { get; private set; } = -1;
    public int Front { get; private set; } = -1;
    public int Rear { get; private set; } = -1;

    public int Count
    {
        get {
            if (Front <= Rear) return Rear - Front;
            else return (Capacity + 1) - Front + Rear;
        }
    }

    public int ListCount { get { return List.Count; } }

    private bool IsEmpty { get { return Front == Rear; } }
    private bool IsFull
    {
        get {
            if (Front < Rear) return (Rear - Front) == Capacity;
            else return (Rear + 1) == Front;
        }
    }

    #endregion

    public CircularQueue(int capacity)
    {
        Initialize(capacity);
    }

    ~CircularQueue()
    {
        if (List != null)
        {
            for (int i = 0; i < List.Count; i++)
                List[i] = null;

            List = null;
        }
    }

    #region PUBLIC METHOD

    public void Enqueue(T data)
    {
        if (IsFull)
            Capacity = Remalloc(MALLOC_SIZE);

        int position = 0;

        if (Rear.Equals(Capacity)) { Rear = 1; position = 0; }
        else { position = Rear; Rear++; }

        List[position] = data;
    }

    public T Dequeue()
    {
        if (IsEmpty)
            return null;

        int position = Front;

        if (Front.Equals(Capacity - 1)) Front = 0;
        else Front++;

        return List[position];
    }

    public T Get(int index)
    {
        return List.Count > index ? List[index] : null;
    }

    #endregion

    #region PRIVATE METHOD

    private void Initialize(int capacity)
    {
        Capacity = capacity;

        if (List == null)
            List = new List<T>();

        for (int i = 0; i < Capacity + 1; i++)
            List.Add(new T());

        Front = 0;
        Rear = capacity;
    }

    private int Remalloc(int size)
    {
        int reSize = Capacity + size;

        for (int i = 0; i < size; i++)
            List.Add(new T());

        return reSize;
    }

    #endregion
}
