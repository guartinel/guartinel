package sysment.guartinelandroidsystemwatcher.persistance.DBO;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by moqs_the_one on 2017.08.15..
 */

public class ServerInstances extends ArrayList<ServerInstance> {


    public ServerInstances fromJSON(String json) {
        Gson gson = new Gson();
        Type type = new TypeToken<List<ServerInstance>>() {
        }.getType();
        ArrayList<ServerInstance> result = gson.fromJson(json, type);
        for (ServerInstance newItem : result) {
            add(newItem);
        }
        return  this;
    }

    public String toJSON() {
        Gson gson = new Gson();
        Type type = new TypeToken<List<ServerInstance>>() {
        }.getType();
        List<ServerInstance> items = new ArrayList<ServerInstance>();
        for (int i = 0; i < super.size(); i++) {
            items.add(super.get(i));
        }
        String s = gson.toJson(items, type);
        return s;
    }
}
