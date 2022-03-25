using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rectangle
{
  public int x;
  public int y;
  public int w;
  public int h;

  public Rectangle()
  {
    x = 1;
    y = 1;
    w = 1;
    h = 1;
  }

  public Rectangle(int _x, int _y, int _w, int _h)
  {
    x = _x;
    y = _y;
    w = _w;
    h = _h;
  }
};

public class BSPPoint
{
  public int x, y;

  public BSPPoint()
  {
    x = 0;
    y = 0;
  }

  public BSPPoint(int _x, int _y)
  {
    x = _x;
    y = _y;
  }
};

class Leaf
{
  private const int MIN_LEAF_SIZE = 10;

  public int y;
  public int x;
  public int width;
  public int height; // the position and size of this Leaf

  public Leaf leftChild = null; // the Leaf's left child Leaf
  public Leaf rightChild = null; // the Leaf's right child Leaf
  public Rectangle room = new Rectangle(); // the room that is inside this Leaf
                                           //var halls:Vector.; // hallways to connect this Leaf to other Leafs

  bool hasRooms;

  //std::vector<Rectangle> halls;
  public List<Rectangle> halls = new List<Rectangle>();


  public Leaf(int X, int Y, int Width, int Height)
  {

    leftChild = null;
    rightChild = null;

    // initialize our leaf
    init(X, Y, Width, Height);
  }

  public void init(int X, int Y, int Width, int Height)
  {
    // initialize our leaf
    leftChild = null;
    rightChild = null;
    x = X;
    y = Y;
    width = Width;
    height = Height;

    if (height < 3) height = 3;
    if (width < 3) width = 3;

  }

  Leaf()
  {
    leftChild = null;
    rightChild = null;
    x = 0;
    y = 0;
    width = 3;
    height = 3;
  }

  public void clear()
  {

    halls.Clear();


    if (leftChild != null)
    {
      leftChild.clear();
    }
    if (rightChild != null)
    {
      rightChild.clear();
    }

    //delete leftChild;
    //delete rightChild;
    leftChild = null;
    rightChild = null;


  }


  ~Leaf()
  {

    //printf("d");
  }


  public bool split()
  {
    // begin splitting the leaf into two children
    if (leftChild != null || rightChild != null)
      return false; // we're already split! Abort!

    // determine direction of split
    // if the width is >25% larger than height, we split vertically
    // if the height is >25% larger than the width, we split horizontally
    // otherwise we split randomly

    bool splitH = false;
    if (Random.Range(0, 100) > 50)
      splitH = true;

    if (width > height && (float)(width / height) >= 1.25f)
      splitH = false;
    else if (height > width && (float)(height / width) >= 1.25f)
      splitH = true;

    int max = (splitH ? height : width) - MIN_LEAF_SIZE; // determine the maximum height or width

    if (max <= MIN_LEAF_SIZE)
      return false; // the area is too small to split any more...

    int k = max - MIN_LEAF_SIZE;
    int splitti = MIN_LEAF_SIZE + (Random.Range(0, k - 1)); // determine where we're going to split

    // create our left and right children based on the direction of the split
    if (splitH)
    {
      leftChild = new Leaf(x, y, width, splitti);
      rightChild = new Leaf(x, y + splitti, width, height - splitti);
    }
    else
    {
      leftChild = new Leaf(x, y, splitti, height);
      rightChild = new Leaf(x + splitti, y, width - splitti, height);
    }

    return true; // split successful!
  }


  public void createRooms()
  {
    // this function generates all the rooms and hallways for this Leaf and all of its children.
    if (leftChild != null || rightChild != null)
    {
      hasRooms = false;

      // this leaf has been split, so go into the children leafs
      if (leftChild != null)
      {
        leftChild.createRooms();
      }
      if (rightChild != null)
      {
        rightChild.createRooms();
      }

      if (leftChild != null && rightChild != null)
      {
        createHall(leftChild.getRoom(), rightChild.getRoom());
        //printf("A");
      }

    }
    else
    {
      // this Leaf is the ready to make a room
      // the room can be between 3 x 3 tiles to the size of the leaf - 2.
      int _ww = 3 + (int)Random.Range(0, width - 3);
      int _hh = 3 + (int)Random.Range(0, height - 3);
      BSPPoint roomSize = new BSPPoint(_ww, _hh);



      // place the room within the Leaf, but don't put it right
      // against the side of the Leaf (that would merge rooms together)
      //roomPos = new Point(Registry.randomNumber(1, width - roomSize.x - 1), Registry.randomNumber(1, height - roomSize.y - 1));

      _ww = Random.Range(0, roomSize.x - width - 2);
      _hh = Random.Range(0, roomSize.y - height - 2);

      BSPPoint roomPos = new BSPPoint(_ww, _hh);

      //room = new Rectangle(x + roomPos.x, y + roomPos.y, roomSize.x, roomSize.y);
      room.x = x + roomPos.x;
      room.y = y + roomPos.y;
      room.w = roomSize.x;
      room.h = roomSize.y;
      hasRooms = true;
    }
  }


  Rectangle getRoom()
  {
    // iterate all the way through these leafs to find a room, if one exists.
    if (hasRooms)
      return room;
    else
    {
      Rectangle lRoom = null;
      Rectangle rRoom = null;

      if (leftChild != null)
      {
        lRoom = leftChild.getRoom();
      }
      if (rightChild != null)
      {
        rRoom = rightChild.getRoom();
      }
      if (lRoom == null && rRoom == null)
        return null;
      else if (rRoom == null)
        return lRoom;
      else if (lRoom == null)
        return rRoom;
      else if (Random.Range(0, 100) > 50)
        return lRoom;
      else
        return rRoom;
    }
  }


  void createHall(Rectangle l, Rectangle r)
  {
    // now we connect these two rooms together with hallways.
    // this looks pretty complicated, but it's just trying to figure out which point is where and then either draw a straight line, or a pair of lines to make a right-angle to connect them.
    // you could do some extra logic to make your halls more bendy, or do some more advanced things if you wanted.

    BSPPoint point1 = new BSPPoint(l.x + 1 + (Random.Range(0, l.w - 3)), l.y + 1 + (Random.Range(0, l.h - 3)));
    BSPPoint point2 = new BSPPoint(r.x + 1 + (Random.Range(0, l.w - 3)), r.y + 1 + (Random.Range(0, l.h - 3)));

    int w = point2.x - point1.x;
    int h = point2.y - point1.y;


    if (w < 0)
    {
      if (h < 0)
      {
        if (Random.Range(0, 100) < 50)
        {
          halls.Add(new Rectangle(point2.x, point1.y, Mathf.Abs(w), 0));
          halls.Add(new Rectangle(point2.x, point2.y, 0, Mathf.Abs(h)));
        }
        else
        {
          halls.Add(new Rectangle(point2.x, point2.y, Mathf.Abs(w), 0));
          halls.Add(new Rectangle(point1.x, point2.y, 0, Mathf.Abs(h)));
        }
      }
      else if (h > 0)
      {
        if (Random.Range(0, 100) < 50)
        {
          halls.Add(new Rectangle(point2.x, point1.y, Mathf.Abs(w), 0));
          halls.Add(new Rectangle(point2.x, point1.y, 0, Mathf.Abs(h)));
        }
        else
        {
          halls.Add(new Rectangle(point2.x, point2.y, Mathf.Abs(w), 0));
          halls.Add(new Rectangle(point1.x, point1.y, 0, Mathf.Abs(h)));
        }
      }
      else // if (h == 0)
      {
        halls.Add(new Rectangle(point2.x, point2.y, Mathf.Abs(w), 0));
      }
    }
    else if (w > 0)
    {
      if (h < 0)
      {
        if (Random.Range(0, 100) > 50)
        {
          halls.Add(new Rectangle(point1.x, point2.y, Mathf.Abs(w), 0));
          halls.Add(new Rectangle(point1.x, point2.y, 0, Mathf.Abs(h)));
        }
        else
        {
          halls.Add(new Rectangle(point1.x, point1.y, Mathf.Abs(w),0));
          halls.Add(new Rectangle(point2.x, point2.y, 0, Mathf.Abs(h)));
        }
      }
      else if (h > 0)
      {
        if (Random.Range(0, 100) < 50)
        {
          halls.Add(new Rectangle(point1.x, point1.y, Mathf.Abs(w), 0));
          halls.Add(new Rectangle(point2.x, point1.y, 0, Mathf.Abs(h)));
        }
        else
        {
          halls.Add(new Rectangle(point1.x, point2.y, Mathf.Abs(w), 0));
          halls.Add(new Rectangle(point1.x, point1.y, 0, Mathf.Abs(h)));
        }
      }
      else // if (h == 0)
      {
        halls.Add(new Rectangle(point1.x, point1.y, Mathf.Abs(w), 0));
      }
    }
    else // if (w == 0)
    {
      if (h < 0)
      {
        halls.Add(new Rectangle(point2.x, point2.y, 0, Mathf.Abs(h)));
      }
      else if (h > 0)
      {
        halls.Add(new Rectangle(point1.x, point1.y, 0, Mathf.Abs(h)));
      }
    }
  }

}



public class MapTile
{
  public bool canWalk;
  public bool explored;
  public bool secretWall;
  public bool inFov;
  public bool createMe;

  public MapTile()
  {
    canWalk = false;
    explored = false;
    secretWall = false;
    inFov = false;
    createMe = false;

  }
}

