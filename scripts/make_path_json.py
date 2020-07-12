





lat_data_file = open("data/path.csv")
data = lat_data_file.readlines()

loc_json = open("path_data.json", 'w')
loc_json.write('{\n\t"AllData": \n\t[')

first = True

for line in data[1:]:
    if not first:
        loc_json.write(",\n")
    else:
        first = False
    line_data = line.strip().split(',')
    loc_json.write(" \n\t\t{\n\t\t\"Data\": \n\t\t[\n\t\t\t")
    for i in range(len(line_data)):
    	if line_data[i] != 'nan':
    		temp = line_data[i]
    		if temp.endswith('.0'):
    			temp = temp[:-2]
    		loc_json.write("\"" + temp + "\"")
    	if i < len(line_data) - 1 and line_data[i+1] != 'nan':
    		loc_json.write(", ")
    loc_json.write("\n\t\t\t]\n\t\t}")
    # print (line_data)
    # print (line_data)
    # loc_json.write(" \n\t\t{\n\t\t\"Data\": \n\t\t\t[\n\t\t\t\"" + line_data[1] + "\", \"" + line_data[2] +
    #  "\", \""  + line_data[3] + "\"\n\t\t\t]\n\t\t}")

loc_json.write('\n\t]\n}')
loc_json.close()

lat_data_file.close()
