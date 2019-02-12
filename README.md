# WorkRecordPlugin (ADAPT 1.2.0)
ADAPT plugin exporting workRecords in JSON files


From
![ADAPT](https://i.imgur.com/1HWzEz6.png)
To
![JSON](https://i.imgur.com/PV0eZhR.png)

Currently only exporting from ADM to JSON-files USING ADAPT 1.2.0


- Export properties:
  - Maximum depth of mapping
  - Data anonymization (Need to explain what exactly this means!)
  - ReferenceIds of which WorkRecords to export only
- Version File/Header containing additional metadata 
  - Version plugin
  - Version ADAPT
  - Date of conversion
  


To Do:
- ADAPT 2.0!
- ISOv4Plugin
- Export properties: 
  - More simplified  
  - Compression option
    - Protobuf?
    - ZipUtil?  
  - ReferenceIds of which WorkRecords to export only
  - ...  
- ProductDto
- UserRoleDto
- CompoundIdentifiers!
  - In JSON-file itself or seperated JSON-file such as a LinkList (~ISO11783-10)
- Import!

  
