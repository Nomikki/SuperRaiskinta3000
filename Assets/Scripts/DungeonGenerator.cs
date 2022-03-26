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

  public float GetCenterX()
  {
    return x + (w / 2);
  }

  public float GetCenterY()
  {
    return y + (h / 2);
  }
};


public class BspNode : Rectangle
{
  public BspNode A = null;
  public BspNode B = null;
  public Rectangle leaf;

  public BspNode(Rectangle _leaf) : base(_leaf.x, _leaf.y, _leaf.w, _leaf.h)
  {
    this.leaf = _leaf;
  }

  public List<BspNode> GetLeafs()
  {
    List<BspNode> l = new List<BspNode>();
    if (A == null || B == null)
    {
      BspNode c = new BspNode(leaf);
      l.Add(c);
    }
    else
    {
      List<BspNode> lA = A.GetLeafs();
      foreach (BspNode aa in lA)
        l.Add(aa);

      List<BspNode> lB = B.GetLeafs();
      foreach (BspNode bb in lB)
        l.Add(bb);
    }

    return l;
  }
};

public class BspGenerator
{
  public List<Rectangle> rooms;
  public List<Rectangle> doorPlaces;
  public List<Rectangle> tempRooms;

  public BspNode tree;

  int maxLevel = 0;
  Rectangle rootContainer;

  int rows = 0;
  int cols = 0;

  public BspGenerator(int x, int y, int w, int h, int maxLevel)
  {
    rootContainer = new Rectangle(x + 1, y + 1, w - 2, h - 2);
    rows = h;
    cols = w;

    this.maxLevel = maxLevel;
    rooms = new List<Rectangle>();
    rooms.Clear();

    doorPlaces = new List<Rectangle>();
    doorPlaces.Clear();

    tempRooms = new List<Rectangle>();
    tempRooms.Clear();

    tree = Devide(rootContainer, 0);

    List<BspNode> bl = tree.GetLeafs();
    foreach (BspNode b in bl)
      rooms.Add(b);

    CreateRooms();
    ConnectRooms(tree);
  }

  void CreateRooms()
  {
    foreach (Rectangle room in rooms)
    {
      int w = (int)Random.Range(room.w * 0.5f, room.w * 0.9f);
      int h = (int)Random.Range(room.h * 0.5f, room.h * 0.9f);
      int x = (int)Random.Range(room.x, room.x + room.w - w);
      int y = (int)Random.Range(room.y, room.y + room.h - h);

      Rectangle rect = new Rectangle(x, y, x + w, y + h);
      this.tempRooms.Add(rect);
    }
  }

  bool ConnectRooms(BspNode node)
  {
    if (node.A == null || node.B == null)
      return false;
    ConnectRooms(node.A);
    ConnectRooms(node.B);

    return true;
  }

  (Rectangle a, Rectangle b) RandomSplit(Rectangle container)
  {
    Rectangle r1;
    Rectangle r2;

    bool splitVertical = false;

    if (container.w > container.h && container.w / container.h > 0.05)
    {
      splitVertical = true;
    }
    else
    {
      splitVertical = false;
    }

    if (splitVertical)
    {
      int w = (int)Random.Range(container.w * 0.3f, container.w * 0.6f);
      r1 = new Rectangle(container.x, container.y, w, container.h);
      r2 = new Rectangle(container.x + w, container.y, container.w - w, container.h);
    }
    else
    {
      int h = (int)Random.Range(container.w * 0.3f, container.w * 0.6f);
      r1 = new Rectangle(container.x, container.y, container.w, h);
      r2 = new Rectangle(container.x, container.y + h, container.w, container.h - h);
    }

    return (r1, r2);
  }

  BspNode Devide(Rectangle container, int level)
  {
    BspNode root = new BspNode(container);

    if (level < maxLevel)
    {
      (Rectangle a, Rectangle b) = RandomSplit(container);
      root.A = this.Devide(a, level + 1);
      root.B = this.Devide(b, level + 1);
    }

    return root;

  }
};



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
  }
}

