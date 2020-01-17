# Genetic Programming
Genetic programming was derived from the model of biological evolution. At first we generate random trees that holds p, r, h strings and " * , / , +, -" operations as string. As you know, area of the cylinder is 2xpxrxh and volume is pxr^2xh. Then we randomly crossover tree's genes that generated randomly. After crossovers our radiation session starts! We randomly mutate our trees genes. At last, we kill all the trees that are under some threshold. Repeat these steps until there are few trees left. Finally, we can print all the remaining trees!

## Main Program
![9](https://user-images.githubusercontent.com/30238276/72573035-85ab3180-38d5-11ea-8ad5-3399accd7a4c.PNG)

## Finding the area of the cylinder formula
![10](https://user-images.githubusercontent.com/30238276/72573167-c99e3680-38d5-11ea-869d-4ef89aedc83f.PNG)
### Wait!? This looks like wrong! but actually it is not. Program traverse our trees in infix order and we do not have parenthesis, so actually this is "pxhx(r+r) = pxhx2r"

## Now let's try to find the volume of the cylinder formula
![11](https://user-images.githubusercontent.com/30238276/72573331-5ea12f80-38d6-11ea-9c55-6af2c10f7114.PNG)
### Again, it looks wrong at first glance but it is NOT! We actually multiply h by h then divide by h, this is equivalent to h.
