
import pandas as pd
import numpy as np
from math import sin,cos,pi
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D


data = pd.read_csv('../UPS 3162/small_lat.csv')

data = data.drop_duplicates(subset='Facility Latitude',keep='first',inplace=False)
data.index = range(len(data))
dic = {}
x = []
y = []
z = []
for i in range(len(data)):
	slic = data.loc[i]['SLIC']
	latitude = data.loc[i]['Facility Latitude']
	longitude = data.loc[i]['Facility Longitude']
	x.append(0.5 * cos(longitude * pi / 180) * cos(latitude * pi / 180))
	y.append(0.5 * sin(latitude * pi / 180))
	z.append(0.5 * sin(longitude * pi / 180) * cos(latitude * pi / 180))
	print (x,y,z)

ax = plt.figure()
ax = Axes3D(ax)
ax.scatter(x,y,z,s=10)
plt.show()