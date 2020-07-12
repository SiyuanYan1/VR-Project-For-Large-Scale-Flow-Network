

# import pandas as pd

# data = pd.read_csv("data/slc2.csv")

# # print (data['SLIC'])
# needed = data[['SLIC','SLIC_State']]
# print (needed)

# needed.to_csv("data/fullstate.csv")


lat_data_file = open("data/fullstate.csv")
lat_data_file.readline()

loc_json = open("fullstate.json", 'w')
loc_json.write('{\n\t"AllData": \n\t[ \n\t\t{\n\t\t"Year": 2019, \n\t\t"Data": \n\t\t\t[\n')

first = True

for line in lat_data_file:
    if not first:
        loc_json.write(",\n")
    else:
        first = False
    line_data = line.strip().split(',')
    # print (line_data)
    loc_json.write("\t\t\t\"" + line_data[1] + "\", \"" + line_data[2] +
     "\"")

loc_json.write('\n\t\t\t]\n\t\t}\n\t]\n}')
loc_json.close()

lat_data_file.close()
