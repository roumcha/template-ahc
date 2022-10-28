def main():
  # ---------- lib --------------
  from sys import stdin, stderr, setrecursionlimit
  from collections import deque
  setrecursionlimit(1000500)
  _d, _r = deque(), stdin.readline

  def cin() -> str:
    while not _d:
      _d.extend(_r().split())
    return _d.popleft()
  # ---------- end lib ----------

  print(0)

  return


if __name__ == '__main__':
  main()
