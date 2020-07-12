import csv  
import json  
import pandas as pd
# Open the CSV  
f = open( 'arc_small.csv', 'rU' )  
# Change each fieldname to the appropriate field name. I know, so difficult.  
reader = csv.DictReader( f, fieldnames = ( "SLIC","ox","oy","OSLIC","DSLIC","VOL","dx","dy","VOL_LEVEL=" ))  
# Parse the CSV into JSON  
out = json.dumps( [ row for row in reader ] )  
print ("JSON parsed!"  )
# Save the JSON  
f = open( 'vol_parsed.json', 'w')  
f.write(out)  
print ("JSON saved!"  )