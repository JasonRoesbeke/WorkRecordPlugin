# WorkRecordPlugin (ADAPT 2.0.2)
ADAPT plugin exporting workRecords in JSON files


From
![ADAPT](https://i.imgur.com/1HWzEz6.png)
To
![JSON](https://i.imgur.com/PV0eZhR.png)

Currently only exporting from ADM to JSON-files USING ADAPT 1.2.0


- Export properties:
- [x] Maximum depth of mapping
- [x] Data anonymization 
  - Currently Data is NOT truly anonymised, e.g.: 
    - Timestamps are not changed
    - Distance of moving coordinates of spatial records is randomly chosen between 30 km and 80 km but this may not be enough!
- [x] ReferenceIds of which WorkRecords to export only
  - Version File/Header containing additional metadata 
- [x] Version plugin
- [x] Version ADAPT
- [x] Date of conversion
  


To Do:
- [ ] ISOv4Plugin Support
- [ ] Export properties: 
  - More simplified  
  - Compression option
    - Protobuf?
    - ZipUtil?  
  - ...  
- [ ] ProductDto
- [ ] UserRoleDto
- [ ] CompoundIdentifiers!
  - In JSON-file itself or seperated JSON-file such as a LinkList (~ISO11783-10)
- [ ] Import!

  
