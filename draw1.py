import matplotlib.pyplot as plt

# Read points from file
points = []
with open('.\\Input\\Contour.txt', 'r') as f:
    next(f)
    for line in f:
        x, y = line.strip().split(' ')
        points.append((float(x), float(y)))

# Draw points
x_vals = [p[0] for p in points]
x_vals.append(x_vals[0])
y_vals = [p[1] for p in points]
y_vals.append(y_vals[0])
plt.plot(x_vals, y_vals, 'bo-')

# Draw lines connecting neighboring points
for i in range(len(points) - 1):
    p1 = points[i]
    p2 = points[i+1]
    plt.plot([p1[0], p2[0]], [p1[1], p2[1]], 'r')

plt.show()
