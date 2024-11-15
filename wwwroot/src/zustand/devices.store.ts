import {create} from 'zustand';

interface Device {
  devices: any[];
  addDevice: (dev: any) => void;
  removeDevice: (dev: any) => void;
}

const useDeviceStore = create<Device>((set) => ({
  devices: [],
  addDevice: (dev) => set((state) => ({ devices: [...state.devices, dev] })),
  removeDevice: (dev) => set((state) => ({ devices: state.devices.filter((item) => dev.id !== item.id) })),
  insertDevice: (dev, index) => set((state) => ({ devices: [...state.devices.slice(0, index), dev, ...state.devices.slice(index)] })),
  clearDevices: () => set({ devices: [] }),
}));

export default useDeviceStore;
