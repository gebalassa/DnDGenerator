using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCTrainer : ScriptableObject
{
    // TRAINING
    // Get all images from ImageDatabase
    // Create dictionary to save tile frequency, associated tiles and their dir.
    // Inside , use training data:
    // --From each training image, check each tile.
    // --Add 1 to its frequency.    
    // FOR EACH TILE
    // For each training tile, check surrounding tiles.
    // If any of those tiles is new to the current one,
    // add to associated tiles with their dir.
    
    // RECOMMENDATION TILE WHEN COLLAPSING
    // To get recommendation (when collapsing least entropy tile),
    // use the frequencies of each remaining possibility:
    // -- Order remaining from least to most frequent.
    // -- Random number from 0 to SUM OF THEIR FREQUENCIES.
    // -- Use aggregated frequency trick to choose.
}
