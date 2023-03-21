import matplotlib.pyplot as plt

# Define the colors for different labels and line numbers
point_colors = {'first': 'red', 'second': 'blue', 'inner': 'black', 'outer': 'grey'}
line_colors = {1: 'red', 2: 'blue'}

# Read the point data from a file
with open('DrawGrid.txt') as f:
    point_data = [line.strip().split() for line in f]

# Extract the coordinates and labels from the point data
x = [float(d[0]) for d in point_data]
y = [float(d[1]) for d in point_data]
labels = [d[2] for d in point_data]

# Read the line data from a file
with open('.\\Input\\Tests\\test_conditions.txt', encoding='utf-8-sig') as f:
    line_data = [line.strip().split() for line in f]

# Extract the coordinates and numbers from the line data
x0 = [float(d[0]) for d in line_data]
y0 = [float(d[1]) for d in line_data]
x1 = [float(d[2]) for d in line_data]
y1 = [float(d[3]) for d in line_data]
numbers = [int(d[5]) for d in line_data]

# Plot the points and lines with different colors based on their labels and numbers
fig, ax = plt.subplots()
for xi, yi, label in zip(x, y, labels):
    color = point_colors.get(label, 'black')
    ax.scatter(xi, yi, color=color)

for x0i, y0i, x1i, y1i, num in zip(x0, y0, x1, y1, numbers):
    color = line_colors.get(num, 'black')
    ax.plot([x0i, x1i], [y0i, y1i], color=color)

plt.show()

