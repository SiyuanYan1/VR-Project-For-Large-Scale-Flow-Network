point_size = "0.005"

lat_data_file = open("small_lat.csv")
lat_data_file.readline()

loc_json = open("loc_data.json", 'w')
loc_json.write('{\n\t"AllData": \n\t[ \n\t\t{\n\t\t"Year": 2019, \n\t\t"Data": \n\t\t\t[\n')

first = True

for line in lat_data_file:
    if not first:
        loc_json.write(",\n")
    else:
        first = False
    line_data = line.strip().split(',')
    loc_json.write("\t\t\t" + line_data[1] + ", " + line_data[2] + ", " + point_size)

loc_json.write('\n\t\t\t]\n\t\t}\n\t]\n}')
loc_json.close()

lat_data_file.close()
